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

        readonly KestrelServerOptions options;

        readonly LoggerFactory logger;

        // the embedded server
        readonly KestrelServer server;

        IPAddress inaddr;

        int inport;

        //
        // MSG POLLER / CONNECTOR

        // load messages        
        readonly Roll<MsgLoader> loaders;


        // topics subscribed by this microservice
        public Roll<MsgSubscription> Subscriptions { get; } = new Roll<MsgSubscription>(16);

        private Thread scheduler;

        readonly Roll<MsgPoller> pollers;


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

            ParseAddress(cfg.Private, out inaddr, out inport);

            CreateMsgTables();
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


        static bool ParseAddress(string addr, out IPAddress ipaddr, out int port)
        {
            port = 0;
            string sip = addr;
            int colon = addr.LastIndexOf(':');
            if (colon != -1)
            {
                sip = addr.Substring(0, colon);
                string sport = addr.Substring(colon + 1);
                port = Int32.Parse(sport);
            }
            ipaddr = IPAddress.Parse(sip);
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
        public async Task ProcessRequestAsync(HttpContext hc)
        {
            // dispatch the context accordingly
            ConnectionInfo ci = hc.Connection;
            IPAddress ip = ci.LocalIpAddress;
            int port = ci.LocalPort;

            WebContext wc = (WebContext)hc;
            if (port == inport && ip.Equals(inaddr))
            {
                // mq handling or action handling
                if (hc.Request.Path.Equals("*"))
                {
                    // msg queue
                    Poll(wc);
                }
                else
                {
                    Handle(wc.Request.Path.Value.Substring(1), wc);
                }
            }
            else
            {
                // check security token (authentication)

                // handling
                Handle(wc.Request.Path.Value.Substring(1), wc);
            }

            if (wc.Content != null)
            {
                await wc.WriteContentAsync();
            }
            if (wc.IsSuspended)
            {
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            WebContext wc = (WebContext)context;
            wc.Dispose();
        }

        public void Start()
        {
            // start the server
            //
            server.Start(this);

            var urls = server.Features.Get<IServerAddressesFeature>().Addresses;

            Console.Write(Key);
            Console.Write(" started (");
            int i = 0;
            foreach (var url in urls)
            {
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write(url);
                i++;
            }
            Console.Write(")");
            Console.WriteLine();
        }

        public void StopApplication()
        {
        }

        /// <summary>
        /// Poll message from database and cache 
        /// </summary>
        /// <param name="wc"></param>
        internal void Poll(WebContext wc)
        {
        }

        public CancellationToken ApplicationStarted { get; set; }

        public CancellationToken ApplicationStopping { get; set; }

        public CancellationToken ApplicationStopped { get; set; }


        internal void Schedule()
        {
            while (true)
            {
                for (int i = 0; i < pollers.Count; i++)
                {
                    MsgPoller conn = pollers[i];

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

        public static void Run(params WebService[] services)
        {
            foreach (WebService svc in services)
            {
                svc.Start();
            }

            var token = new CancellationToken();

            token.Register(
                state => { ((IApplicationLifetime)state).StopApplication(); },
                Lifetime
            );

            Lifetime.ApplicationStopping.WaitHandle.WaitOne();
        }
    }
}