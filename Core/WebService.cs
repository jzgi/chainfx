using System;
using System.Collections.Generic;
using System.Net;
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
    public abstract class WebService : WebHub, IHttpApplication<HttpContext>
    {
        //
        // SERVER
        //

        readonly KestrelServerOptions options;

        readonly LoggerFactory logger;

        // the embedded server
        readonly KestrelServer server;

        // private address
        string priaddr;

        //
        // MESSAGING
        //

        // load messages        
        internal Roll<MsgLoader> MLoaders { get; } = new Roll<MsgLoader>(32);

        // topics subscribed by this microservice
        internal Roll<MsgAction> MActions { get; } = new Roll<MsgAction>(16);

        private Thread msgScheduler;

        internal Roll<MsgPoller> MPollers { get; } = new Roll<MsgPoller>(32);


        protected WebService(WebServiceConfig cfg) : base(cfg)
        {
            // init eqc client
            foreach (var ep in cfg.Net)
            {
                //				ParseAddress()
            }

            // create the server instance
            logger = new LoggerFactory();

            options = new KestrelServerOptions();

            server = new KestrelServer(Options.Create(options), Lifetime, logger);
            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            addrs.Add(cfg.Tls ? "https://" : "http://" + cfg.Public);
            addrs.Add("http://" + cfg.Private); // clustered msg queue

            CreateMsgTables();

            priaddr = cfg.Private;

            List<string> net = cfg.Net;
            for (int i = 0; i < net.Count; i++)
            {
                string addr = net[i];
                if (addr.Equals(cfg.Private)) continue;
                if (MPollers == null) MPollers = new Roll<MsgPoller>(net.Count);
                MPollers.Add(new MsgPoller(this, addr));
            }
        }

        internal bool CreateMsgTables()
        {
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
            if (laddr.Equals(priaddr)) // [PROC] private traffic
            {
                // verify the remote addrees 
                string raddr = ci.RemoteIpAddress.ToString();
                if (!MPollers.Contains(raddr))
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

            WebServiceConfig cfg = (WebServiceConfig)Config;

            Console.Write(Key);
            Console.Write(" -> ");
            Console.Write(cfg.Public);
            Console.Write(" / ");
            Console.Write(cfg.Private);
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
                for (int i = 0; i < MPollers.Count; i++)
                {
                    MsgPoller conn = MPollers[i];

                    // schedule
                }
            }
        }

        public DbContext NewDbContext()
        {
            DbConfig cfg = ((WebServiceConfig)Config).Db;
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
    }
}