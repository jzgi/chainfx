using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Greatbone.Core
{
    ///
    /// A service work implements a microservice.
    ///
    public abstract class WebService : WebDirectory, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the globally unique service instance id
        readonly string id;

        readonly KestrelServerOptions options;

        // file logger factory
        readonly LoggerFactory factory;

        // the embedded server
        readonly KestrelServer server;

        // shared content cache
        readonly ContentCache cache;

        // connectivity to the remote peers, for remote call as well as messaging
        readonly Roll<WebReference> references;

        // event hooks
        readonly Roll<WebEvent> events;

        readonly Thread scheduler;


        protected WebService(WebConfig cfg) : base(cfg)
        {
            // adjust configuration
            cfg.Service = this;

            id = (cfg.subkey == null) ? cfg.key : cfg.key + "-" + cfg.subkey;

            // setup logging 
            factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = cfg.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false)
            {
                AutoFlush = true
            };

            // create embedded server instance
            options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), Lifetime, factory);
            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            addrs.Add(cfg.outer);
            addrs.Add(cfg.inner);

            // init event hooks
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebEventContext))
                {
                    WebEvent evt = new WebEvent(this, mi);
                    if (events == null)
                    {
                        events = new Roll<WebEvent>(16);
                    }
                    events.Add(evt);
                }
            }

            // init refers
            Obj refs = cfg.refs;
            if (refs != null)
            {
                for (int i = 0; i < refs.Count; i++)
                {
                    Member mbr = refs[i];
                    string svcid = mbr.Key; // service instance id
                    string addr = mbr;
                    if (references == null)
                    {
                        references = new Roll<WebReference>(refs.Count * 2);
                    }
                    references.Add(new WebReference(svcid, addr));
                }
            }

            InstallEq();

            // init content cache
            cache = new ContentCache(Environment.ProcessorCount * 2, 4096);
        }

        public WebConfig Config => (WebConfig)makectx;

        ///
        /// The service instance id.
        ///
        public string Id => id;

        internal Roll<WebEvent> Events => events;

        internal Roll<WebReference> References => references;

        bool InstallEq()
        {
            if (!Config.db.queue)
            {
                return false;
            }

            // check db
            using (var dc = Service.NewDbContext())
            {
                dc.Execute(@"CREATE TABLE IF NOT EXISTS eq (
                                id serial8 NOT NULL,
                                time timestamp without time zone,
                                topic character varying(20),
                                subkey character varying(10),
                                body bytea,
                                CONSTRAINT eq_pkey PRIMARY KEY (id)
                            ) WITH (OIDS=FALSE)",
                    null
                );
                dc.Execute(@"CREATE TABLE IF NOT EXISTS eu (
                                svcid character varying(20),
                                lastid int8,
                                CONSTRAINT eu_pkey PRIMARY KEY (addr)
                            ) WITH (OIDS=FALSE)",
                    null
                );
            }
            return true;
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }


        ///  
        /// Returns a framework custom context.
        /// 
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new WebActionContext(features);
        }

        /// 
        /// To asynchronously process the request.
        /// 
        public async Task ProcessRequestAsync(HttpContext context)
        {
            WebActionContext ac = (WebActionContext)context;
            HttpRequest req = ac.Request;
            string path = req.Path.Value;
            string targ = path + req.QueryString.Value;

            IContent cont;
            if (ac.IsGetMethod && cache.TryGetContent(targ, out cont)) // check if hit in the cache
            {
                ac.Send(304, cont, true, 0);
            }
            else // handling
            {
                try
                {
                    // authentication
                    string token = null;
                    string hv = ac.Header("Authorization");
                    if (hv != null && hv.StartsWith("Bearer ")) // the Bearer scheme
                    {
                        token = hv.Substring(7);
                        ac.Principal = Principalize(token);
                    }
                    else if (ac.Cookies.TryGetValue("Bearer", out token))
                    {
                        ac.Principal = Principalize(token);
                    }

                    Handle(path.Substring(1), ac);

                    // prepare and send
                    await ac.SendAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    if (e is ParseException)
                    {
                        ac.StatusCode = 304;
                    }
                    else
                    {
                        ERR(e.Message, e); // stacktrace
                    }
                }
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            var ac = (WebActionContext)context;

            // public cache
            IContent cont = ac.Content;
            if (ac.IsCacheable)
            {
                cache.Add(ac.Url, ac.MaxAge, cont);
            }
            else if (cont != null && cont.IsPoolable)
            {
                BufferUtility.Return(cont.ByteBuf); // return response content buffer
            }

           ((WebActionContext)context).Dispose();
        }

        protected virtual IPrincipal Principalize(string token)
        {
            return null;
        }

        internal override void Handle(string relative, WebActionContext ac)
        {
            if ("*".Equals(relative))
            {
                // handle as event
                ForEvents(ac);
            }
            else
            {
                base.Handle(relative, ac);
            }
        }

        public void Start()
        {
            // start the server
            //
            server.Start(this);

            OnStart();

            Console.Write(Key);
            Console.Write(" -> ");
            Console.Write(Config.outer);
            Console.Write(", ");
            Console.Write(Config.inner);
            Console.WriteLine();

            INF("started");
        }

        public DbContext NewDbContext()
        {
            DbConfig cfg = Config.db;
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder()
            {
                Host = cfg.host,
                Database = Key,
                Username = cfg.username,
                Password = cfg.password
            };
            return new DbContext(builder);
        }

        //
        // MESSAGING
        //

        internal WebReference GetReference(string svcid)
        {
            for (int i = 0; i < references.Count; i++)
            {
                WebReference @ref = references[i];
                if (@ref.Key.Equals(svcid)) return @ref;
            }
            return null;
        }


        void ForEvents(WebActionContext ac)
        {
            string svc = ac.Header("service");
            string sub = "";
            int? lastid = ac.HeaderInt("Range");

            using (var dc = NewDbContext())
            {
                if (dc.Query("SELECT * FROM eq WHERE id > @1 AND service = @2 AND sub = @3 LIMIT 120", p => p.Put(lastid.Value)))
                {

                }
                else
                {
                    ac.StatusCode = 204; // no content
                }
            }
        }

        internal void Schedule()
        {
            while (true)
            {
                for (int i = 0; i < References.Count; i++)
                {
                    WebReference conn = References[i];

                    // schedule
                }
            }
        }

        //
        // LOGGING
        //

        // sub controllers are already there
        public ILogger CreateLogger(string name)
        {
            return this;
        }

        // opened writer on the log file
        readonly StreamWriter logWriter;

        public IDisposable BeginScope<T>(T state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel level)
        {
            return (int)level >= Config.logging;
        }


        public void Dispose()
        {
            server.Dispose();

            logWriter.Flush();
            logWriter.Dispose();

            Console.Write(Key);
            Console.WriteLine(".");
        }

        static readonly string[] LVL = { "TRC: ", "DBG: ", "INF: ", "WAR: ", "ERR: " };

        public void Log<T>(LogLevel level, EventId eid, T state, Exception exception, Func<T, Exception, string> formatter)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            logWriter.Write(LVL[(int)level]);

            if (eid.Id != 0)
            {
                logWriter.Write("{");
                logWriter.Write(eid.Id);
                logWriter.Write("} ");
            }

            if (formatter != null) // custom format
            {
                var message = formatter(state, exception);
                logWriter.WriteLine(message);
            }
            else // fixed format
            {
                logWriter.WriteLine(state.ToString());
                if (exception != null)
                {
                    logWriter.WriteLine(exception.StackTrace);
                }
            }
        }

        ///
        /// STATIC
        ///
        static readonly WebLifetime Lifetime = new WebLifetime();


        /// 
        /// Runs a number of web services and block until shutdown.
        /// 
        public static void Run(params WebService[] services)
        {
            using (var cts = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    cts.Cancel();

                    // wait for the Main thread to exit gracefully.
                    eventArgs.Cancel = true;
                };

                // start services
                foreach (WebService svc in services)
                {
                    svc.Start();
                }

                Console.WriteLine("ctrl_c to shut down");

                cts.Token.Register(state =>
                    {
                        ((IApplicationLifetime)state).StopApplication();
                        // dispose services
                        foreach (WebService svc in services)
                        {
                            svc.OnStop();

                            svc.Dispose();
                        }
                    },
                    Lifetime);

                Lifetime.ApplicationStopping.WaitHandle.WaitOne();
            }
        }
    }
}