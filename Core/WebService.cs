using System;
using System.Collections.Generic;
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
    /// <summary>
    /// A service controller that is a module implementation of a microservice.
    /// </summary>
    ///
    public abstract class WebService : WebModule, IHttpApplication<HttpContext>, ILoggerProvider, ILogger, IDisposable
    {
        // SERVER
        //

        readonly KestrelServerOptions options;

        // file logger factory
        readonly LoggerFactory factory;

        // the embedded server
        readonly KestrelServer server;


        // response content cache
        ContentCache cache;

        // MESSAGING
        //

        // load messages from local queue        
        readonly Roll<MsgQueue> queues;

        // hooks of received messages
        readonly Roll<MsgHook> hooks;

        readonly Thread scheduler;

        // poll remote peer servers for subscribed messages
        readonly Roll<WebClient> clients;


        protected WebService(WebConfig cfg) : base(cfg)
        {
            // adjust configuration
            cfg.Service = this;

            // setup logging support
            factory = new LoggerFactory();
            factory.AddProvider(this);
            string logFile = Key + "-" + DateTime.Now.ToString("yyyyMM") + ".log";
            FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false)
            {
                AutoFlush = true
            };

            // create the embedded server instance
            options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), Lifetime, factory);
            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            addrs.Add(cfg.tls ? "https://" : "http://" + cfg.@extern);
            addrs.Add("http://" + cfg.intern);

            // introspect message handler methods
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 1 && pis[0].ParameterType == typeof(MsgContext))
                {
                    MsgHook a = new MsgHook(this, mi);
                    if (hooks == null) hooks = new Roll<MsgHook>(16);
                    hooks.Add(a);
                }
            }

            cache = new ContentCache(1, 1);

            // setup message loaders and pollers
            string[] net = cfg.net;
            for (int i = 0; i < net.Length; i++)
            {
                string addr = net[i];
                if (addr.Equals(cfg.intern)) continue;
                // loader
                if (queues == null) queues = new Roll<MsgQueue>(net.Length * 2);
                queues.Add(new MsgQueue(this, addr));
                // poller
                if (hooks != null)
                {
                    if (clients == null) clients = new Roll<WebClient>(net.Length * 2);
                    clients.Add(new WebClient(this, addr));
                }
            }

            PrepareMsgTables();

        }

        public WebConfig Config => (WebConfig)arg;


        internal Roll<MsgQueue> Queues => queues;

        internal Roll<MsgHook> Hooks => hooks;

        internal Roll<WebClient> Clients => clients;

        bool PrepareMsgTables()
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


        /// <summary>Returns a framework custom context. </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new WebContext(features);
        }

        ///
        /// <summary>To asynchronously process the request.</summary>
        /// <remarks>
        ///
        /// </remarks>
        public async Task ProcessRequestAsync(HttpContext context)
        {
            WebContext wc = (WebContext)context;

            Authenticate(wc);

            // handling
            Handle(wc.Request.Path.Value.Substring(1), wc);

            if (wc.Content != null)
            {
                await wc.WriteContentAsync();
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            ((WebContext)context).Dispose();
        }

        protected abstract bool Authenticate(WebContext wc);


        internal override void Handle(string rsc, WebContext wc)
        {
            if (!CheckAuth(wc)) return;

            int slash = rsc.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                if ("*".Equals(rsc))
                {
                    Peek(wc); // obtain a message
                }
                else
                {
                    wc.Control = this;
                    Do(rsc, wc);
                }
            }
            else // not local then sub & mux
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs != null && subs.TryGet(dir, out sub)) // seek sub first
                {
                    sub.Handle(rsc.Substring(slash + 1), wc);
                }
                else if (varhub == null)
                {
                    wc.StatusCode = 404; // not found
                }
                else
                {
                    varhub.Handle(rsc.Substring(slash + 1), wc, dir); // var = dir
                }
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
            Console.Write(Config.@extern);
            Console.Write(", ");
            Console.Write(Config.intern);
            Console.WriteLine();

            Info("started");
        }

        internal void Schedule()
        {
            while (true)
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    WebClient conn = Clients[i];

                    // schedule
                }
            }
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
        // MESSGING
        //

        internal WebClient FindClient(string service, string part)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                WebClient cli = clients[i];
                if (cli.Key.Equals(service)) return cli;
            }
            return null;
        }
        // sub controllers are already there
        public ILogger CreateLogger(string name)
        {
            return this;
        }



        //
        // MESSAGING
        //

        void Peek(WebContext wc)
        {
            // ensure internally target
            ConnectionInfo ci = wc.Connection;
            string laddr = ci.LocalIpAddress.ToString() + ":" + ci.LocalPort;
            if (laddr.Equals(Config.intern)) // must target the internal address
            {
                wc.StatusCode = 403; // forbidden
                return;
            }

            // queue
            string raddr = ci.RemoteIpAddress.ToString() + ":" + ci.RemotePort;
            MsgQueue loader = queues[raddr];
            loader.Get();
            MsgMessage msg;
            // headers

            // wc.Respond(200, msg);
        }

        public void Dispose()
        {
            server.Dispose();

            logWriter.Flush();
            logWriter.Dispose();

            Console.Write(Key);
            Console.WriteLine("!");
        }

        //
        // LOGGING
        //

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


        static readonly string[] Tags = { "TRACE: ", "DEBUG: ", "INFO: ", "WARNING: ", "ERROR: " };

        public void Log<T>(LogLevel level, EventId eid, T state, Exception exception, Func<T, Exception, string> formatter)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            logWriter.Write(Tags[(int)level]);

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
                    logWriter.WriteLine(exception.ToString());
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