using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public abstract class WebService : WebFolder, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the service instance id
        readonly string id;

        // the embedded server
        readonly KestrelServer server;

        // client connectivity to the remote peers
        readonly Roll<WebClient> refs;

        // event hooks
        readonly Roll<WebEvent> events;

        readonly ResponseCache cache;

        readonly Thread scheduler;

        readonly Thread cleaner;


        protected WebService(WebConfig cfg) : base(cfg)
        {
            // adjust configuration
            cfg.Service = this;

            id = (cfg.shard == null) ? cfg.name : cfg.name + "-" + cfg.shard;

            // setup logging 
            LoggerFactory factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = cfg.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false)
            {
                AutoFlush = true
            };

            // create embedded server instance
            KestrelServerOptions options = new KestrelServerOptions();
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
                    string name = mbr.Name; // service instance id
                    string addr = mbr;
                    if (this.refs == null)
                    {
                        this.refs = new Roll<WebClient>(refs.Count * 2);
                    }
                    this.refs.Add(new WebClient(name, addr));
                }
            }

            InstallEq();

            // init response cache
            if (cfg.cache)
            {
                cache = new ResponseCache(Environment.ProcessorCount * 2, 4096);
            }

            cleaner = new Thread(Clean);

            scheduler = new Thread(Schedule);
        }

        ///
        /// The service instance id.
        ///
        public string Id => id;

        public Roll<WebEvent> Events => events;

        public Roll<WebClient> Clients => refs;

        public WebConfig Config => (WebConfig)context;

        public WebAuth Auth { get; set; }

        internal ResponseCache Cache => cache;

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
                                event character varying(20),
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
                            ) WITH (OIDS=FALSE)"
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

            try
            {
                // authentication
                if (Auth != null)
                {
                    try
                    {
                        Auth.Authenticate(ac);
                    }
                    catch (Exception e)
                    {
                        WAR(e.Message);
                    }
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
                    ac.Status = 400;
                }
                else
                {
                    ERR(e.Message, e); // stacktrace
                }
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            var ac = (WebActionContext)context;

            // public cache
            IContent cont = ac.Content;
            if (cont != null && cont.Poolable)
            {
                BufferUtility.Return(cont.ByteBuffer); // return response content buffer
            }

           ((WebActionContext)context).Dispose();
        }

        internal override void Handle(string relative, WebActionContext ac)
        {
            if ("*".Equals(relative))
            {
                // handle as event
                LoadEvents(ac);
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

            Console.Write(Name);
            Console.Write(" -> ");
            Console.Write(Config.outer);
            Console.Write(", ");
            Console.Write(Config.inner);
            Console.WriteLine();

            INF("started");

            cleaner.Start();

            scheduler.Start();
        }

        public DbContext NewDbContext()
        {
            DbConfig cfg = Config.db;
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder()
            {
                Host = cfg.host,
                Database = Name,
                Username = cfg.username,
                Password = cfg.password
            };
            return new DbContext(Config.shard, builder);
        }

        //
        // CLUSTER
        //

        internal WebClient GetClient(string svcid)
        {
            for (int i = 0; i < refs.Count; i++)
            {
                WebClient cli = refs[i];
                if (cli.Name.Equals(svcid)) return cli;
            }
            return null;
        }


        void LoadEvents(WebActionContext ac)
        {
            string[] events = ac[nameof(events)];
            string shard = ac.Header("shard"); // can be null
            int? lastid = ac.HeaderInt("Range");

            using (var dc = NewDbContext())
            {
                DbSql sql = new DbSql("SELECT * FROM eq WHERE id > @1 AND event IN [");
                for (int i = 0; i < events.Length; i++)
                {
                    if (i > 0) sql.Add(',');
                    sql.Put(events[i]);
                }
                sql.Add(']');
                if (shard != null)
                {
                    sql._("AND (shard IS NULL OR shard = ").Put(shard).Add(')');
                }
                sql._("LIMIT 120");

                if (dc.Query(sql.ToString(), p => p.Put(lastid.Value)))
                {
                    EventsContent cont = new EventsContent(true, 1024 * 1024);
                    while (dc.NextRow())
                    {
                        long id = dc.GetLong();
                        string name = dc.GetString();
                        DateTime time = dc.GetDateTime();
                        ArraySegment<byte>? body = dc.GetBytesSeg();

                        cont.Add(id, name, time, body);
                    }
                    ac.Set(200, cont);
                }
                else
                {
                    ac.Status = 204; // no content
                }
            }
        }

        internal void Schedule()
        {
            while (!stop)
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    WebClient conn = Clients[i];

                    // schedule
                }
            }
        }

        bool stop;

        internal void Clean()
        {
            while (!stop)
            {
                Thread.Sleep(1000);

                int now = Environment.TickCount;


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

            Console.Write(Name);
            Console.WriteLine(".");
        }

        static readonly string[] LVL = { "TRC: ", "DBG: ", "INF: ", "WAR: ", "ERR: ", "CRL: ", "NON: " };

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
        public static void Run(IEnumerable<WebService> services)
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
                var svcs = services as WebService[] ?? services.ToArray();
                foreach (WebService svc in svcs)
                {
                    svc.Start();
                }

                Console.WriteLine("ctrl_c to shut down");

                cts.Token.Register(state =>
                    {
                        ((IApplicationLifetime)state).StopApplication();
                        // dispose services
                        foreach (WebService svc in svcs)
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