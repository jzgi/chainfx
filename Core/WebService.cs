using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
using Microsoft.Extensions.Primitives;
using Npgsql;

namespace Greatbone.Core
{
    ///
    /// A web microservice controller that may contain sub-controllers and/or a multiplexer.
    ///
    /// cache-control -- elimicates redundant requests (max-age) or data queries (not-modified).
    /// response cache -- directly returns shared cached contents
    /// etag -- reduces network I/O with unchanged results
    ///
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


        public WebConfig Config { get; internal set; }

        // MESSAGING
        //

        // load messages from local queue        
        readonly Roll<MsgLoader> mloaders;

        // handling of received messages
        readonly Roll<MsgAction> mactions;

        readonly Thread mscheduler;

        // poll remote peer servers for subscribed messages
        readonly Roll<MsgPoller> mpollers;


        protected WebService(WebConfig cfg) : base(cfg)
        {
            Config = cfg;

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
                    MsgAction a = new MsgAction(this, mi);
                    if (mactions == null) mactions = new Roll<MsgAction>(16);
                    mactions.Add(a);
                }
            }
            // setup message loaders and pollers
            string[] net = cfg.net;
            for (int i = 0; i < net.Length; i++)
            {
                string addr = net[i];
                if (addr.Equals(cfg.intern)) continue;
                // loader
                if (mloaders == null) mloaders = new Roll<MsgLoader>(net.Length * 2);
                mloaders.Add(new MsgLoader(this, addr));
                // poller
                if (mactions != null)
                {
                    if (mpollers == null) mpollers = new Roll<MsgPoller>(net.Length * 2);
                    mpollers.Add(new MsgPoller(this, addr));
                }
            }

            PrepareMsgTables();

        }


        internal Roll<MsgLoader> MsgLoaders => mloaders;

        internal Roll<MsgAction> MsgActions => mactions;

        internal Roll<MsgPoller> MsgPollers => mpollers;

        bool PrepareMsgTables()
        {
            if (!Config.db.MQ)
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
        public async Task ProcessRequestAsync(HttpContext hctx)
        {
            WebContext wc = (WebContext)hctx;

            // dispatch among public/private web actions and msg polling
            ConnectionInfo ci = wc.Connection;
            string laddr = ci.LocalIpAddress.ToString() + ":" + ci.LocalPort;
            if (laddr.Equals(Config.intern)) // [PROC] internal traffic
            {
                // verify the remote addrees 
                string raddr = ci.RemoteIpAddress.ToString();
                if (!MsgPollers.Contains(raddr))
                {
                    wc.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                if (hctx.Request.Path.Equals("*")) // [PROC] msg polling
                {
                    // msg queue
                    PollMsg(wc);
                }
                else // [PROC] private web action
                {
                    Do(wc.Request.Path.Value.Substring(1), wc);
                }
            }
            else // [PROC] public web action
            {
                Authenticate(wc);

                // handling
                Do(wc.Request.Path.Value.Substring(1), wc);
            }

            if (wc.Content != null)
            {
                await wc.WriteContentAsync();
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            ((WebContext)context).Dispose();
        }

        bool Authenticate(WebContext wc)
        {
            StringValues h;
            if (wc.Request.Headers.TryGetValue("Authorization", out h))
            {
                string v = (string)h;
                v.StartsWith("Bearer "); // Bearer scheme
                return true;
            }
            else
            {
                wc.StatusCode = (int)HttpStatusCode.Unauthorized;
                wc.Response.Headers.Add("WWW-Authenticate", "Bearer ");
                return false;
            }

        }

        public void Start()
        {
            // start the server
            //
            server.Start(this);

            var urls = server.Features.Get<IServerAddressesFeature>().Addresses;

            Console.Write(Key);
            Console.Write(" -> ");
            Console.Write(Config.@extern);
            Console.Write(", ");
            Console.Write(Config.intern);
            Console.WriteLine();
        }



        /// <summary>
        /// Poll message from database and cache 
        /// </summary>
        /// <param name="wc"></param>
        internal void PollMsg(WebContext wc)
        {
        }


        internal void Schedule()
        {
            while (true)
            {
                for (int i = 0; i < MsgPollers.Count; i++)
                {
                    MsgPoller conn = MsgPollers[i];

                    // schedule
                }
            }
        }

        public DbContext NewDbContext()
        {
            DbConfig cfg = Config.db;
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder()
            {
                Host = cfg.Host,
                Database = Key,
                Username = cfg.Username,
                Password = cfg.Password
            };
            return new DbContext(builder);
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
                    Console.WriteLine("shutting down...");
                    cts.Cancel();

                    // wait for the Main thread to exit gracefully.
                    eventArgs.Cancel = true;
                };

                foreach (WebService svc in services)
                {
                    svc.Start();
                }

                Console.WriteLine("^C to shut down");

                cts.Token.Register(state =>
                {
                    ((IApplicationLifetime)state).StopApplication();
                }, Lifetime);

                Lifetime.ApplicationStopping.WaitHandle.WaitOne();

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

        public void Dispose()
        {
            logWriter.Flush();
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

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }


        static readonly string[] Hints = { "TRACE", "DEBUG", "INFO", "WARNING", "ERROR" };

        public void Log<T>(LogLevel level, EventId eventId, T state, Exception exception, Func<T, Exception, string> formatter)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            logWriter.Write(Hints[(int)level]);
            if (formatter != null)
            {
                var message = formatter(state, exception);
                logWriter.WriteLine(message);
            }
            else
            {
                logWriter.WriteLine(state.ToString());
                if (exception != null)
                {
                    logWriter.Write(exception.ToString());
                }
            }
        }

    }
}