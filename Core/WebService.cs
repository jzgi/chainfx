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
    public abstract class WebService : WebModule, IHttpApplication<HttpContext>
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
        readonly Set<MsgLoader> loaders;


        // topics subscribed by this microservice
        public Set<MsgSubscribe> Subscribes { get; } = new Set<MsgSubscribe>(16);

        private Thread scheduler;

        readonly Set<MsgPoller> pollers;


        protected WebService(WebServiceConfig cfg) : base(cfg)
        {
            // init eqc client
            foreach (var ep in cfg.Cluster)
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

            // check db
            string sql = "SELECT to_regclass('schema_name.table_name');";
            using (var dc = Service.NewSqlContext())
            {
                dc.QueryA(sql, null);
                // if null

                dc.Execute(@"CREATE TABLE sysmsgs", null);
                dc.Execute(@"CREATE TABLE syslasts", null);
            }

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

            if (wc.Response.Content != null)
            {
                await wc.Response.WriteContentAsync();
            }
            if (wc.IsSuspended)
            {
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            WebContext wc = (WebContext)context;
            // wc.Request.
            //            context.
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


        public void Subscribe(string topic, string filter, MsgDoer doer)
        {
            Subscribes.Add(new MsgSubscribe(topic, filter, doer));
        }


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

        public DbContext NewSqlContext()
        {
            DbConfig dsb = ((WebServiceConfig)Config).Db;
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder()
            {
                Host = dsb.Host,
                Database = Key,
                Username = dsb.Username,
                Password = dsb.Password
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