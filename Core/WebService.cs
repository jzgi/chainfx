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
        // SERVER
        //

        readonly KestrelServerOptions options;

        // file logger factory
        readonly LoggerFactory factory;

        // the embedded server
        readonly KestrelServer server;

        // shared content cache
        readonly ContentCache cache;

        // MESSAGING
        //

        // connectivity to the remote peers, for remote call as well as messaging
        readonly Roll<WebPeer> peers;

        // hooks of received messages
        readonly Roll<WebHook> hooks;

        readonly Thread scheduler;


        protected WebService(WebConfig cfg) : base(cfg)
        {
            // adjust configuration
            cfg.Service = this;

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
            addrs.Add(cfg.tls ? "https://" : "http://" + cfg.outer);
            addrs.Add("http://" + cfg.inner);

            // init message hooks
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebEvent))
                {
                    WebHook h = new WebHook(this, mi);
                    if (hooks == null) hooks = new Roll<WebHook>(16);
                    hooks.Add(h);
                }
            }

            // init peer connections and message queues
            Obj ps = cfg.peers;
            for (int i = 0; i < ps.Count; i++)
            {
                Member mbr = ps[i];

                string name = mbr.Key;
                string addr = mbr;
                if (peers == null)
                {
                    peers = new Roll<WebPeer>(ps.Count * 2);
                }
                peers.Add(new WebPeer(name, addr));
            }
            MsgSetup();

            // init content cache
            cache = new ContentCache(Environment.ProcessorCount * 2, 4096);
        }

        public WebConfig Config => (WebConfig)ctx;

        internal Roll<WebHook> Hooks => hooks;

        internal Roll<WebPeer> Peers => peers;

        bool MsgSetup()
        {
            if (!Config.db.msg)
            {
                return false;
            }

            // check db
            using (var dc = Service.NewDbContext())
            {
                dc.Execute(@"CREATE TABLE IF NOT EXISTS msgq (
                                id serial4 NOT NULL,
                                time timestamp without time zone,
                                topic character varying(20),
                                shard character varying(10),
                                body bytea,
                                CONSTRAINT msgq_pkey PRIMARY KEY (id)
                            ) WITH (OIDS=FALSE)",
                    null
                );
                dc.Execute(@"CREATE TABLE IF NOT EXISTS msgu (
                                addr character varying(45) NOT NULL,
                                lastid int4,
                                CONSTRAINT msgu_pkey PRIMARY KEY (addr)
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


        /// <summary> 
        /// Returns a framework custom context.
        /// </summary>
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new WebContext(features);
        }

        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public async Task ProcessRequestAsync(HttpContext context)
        {
            WebContext wc = (WebContext)context;
            HttpRequest req = wc.Request;
            string path = req.Path.Value;
            string targ = path + req.QueryString.Value;

            IContent cont;
            if (wc.IsGetMethod && cache.TryGetContent(targ, out cont)) // check if hit in the cache
            {
                wc.Send(304, cont, true, 0);
            }
            else // handling
            {
                try
                {
                    Authenticate(wc);

                    Handle(path.Substring(1), wc);

                    // prepare and send
                    await wc.SendAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    if (e is ParseException)
                    {
                        wc.StatusCode = 304;
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
            ((WebContext)context).Dispose();
        }

        void Authenticate(WebContext wc)
        {
            string hv = wc.Header("Authorization");
            if (hv == null) return;
            if (hv.StartsWith("Bearer ")) // the Bearer scheme
            {
                string token = hv.Substring(7);
                IPrincipal prin = Fetch(true, token);
                if (prin != null)
                {
                    wc.Principal = prin; // success
                }
            }
            else if (hv.StartsWith("Digest ")) // the Digest scheme
            {
                FieldParse fp = new FieldParse(hv);
                string username = fp.Parameter("username=");
                string realm = fp.Parameter("realm=");
                string nonce = fp.Parameter("nonce=");
                string uri = fp.Parameter("uri=");
                string response = fp.Parameter("response=");
                // obtain principal
                IPrincipal prin = Fetch(false, username);
                if (prin != null)
                {
                    // A2 = Method ":" digest-uri-value
                    string HA2 = StrUtility.MD5(wc.Method + ':' + uri);
                    // request-digest = KD ( H(A1), unq(nonce-value) ":" H(A2) ) >
                    string digest = StrUtility.MD5(prin.Credential + ':' + nonce + ':' + HA2);
                    if (digest.Equals(response)) // matched
                    {
                        wc.Principal = prin; // success
                    }
                }
            }
        }

        protected virtual IPrincipal Fetch(bool token, string idstr)
        {
            return null;
        }


        internal override void Handle(string relative, WebContext wc)
        {
            if ("*".Equals(relative))
            {
                // handle messaging
                Peek(wc);
            }
            else
            {
                base.Handle(relative, wc);
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

        internal WebPeer FindClient(string service)
        {
            for (int i = 0; i < peers.Count; i++)
            {
                WebPeer cli = peers[i];
                if (cli.Key.Equals(service)) return cli;
            }
            return null;
        }


        void Peek(WebContext wc)
        {
            // ensure internally target
            ConnectionInfo ci = wc.Connection;
            string laddr = ci.LocalIpAddress.ToString() + ":" + ci.LocalPort;
            if (laddr.Equals(Config.inner)) // must target the internal address
            {
                wc.StatusCode = 403; // forbidden
                return;
            }

            // queue
            string raddr = ci.RemoteIpAddress.ToString() + ":" + ci.RemotePort;
            // if (cache.Count > 0)
            // {
            //     item = cache.Dequeue();
            // }
            // else
            // {
            //     using (var dc = service.NewDbContext())
            //     {
            //         dc.Query("SELECT * FROM mqueue WHERE id > @lastid AND ", p=>p.Put(last));
            //     }
            // }

            // wc.Respond(200, msg);
        }

        internal void Schedule()
        {
            while (true)
            {
                for (int i = 0; i < Peers.Count; i++)
                {
                    WebPeer conn = Peers[i];

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

        public void Log<T>(LogLevel level, EventId eid, T state, Exception exception,
            Func<T, Exception, string> formatter)
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


        /// <summary>
        /// Runs a number of web services and block until shutdown.
        /// </summary>
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