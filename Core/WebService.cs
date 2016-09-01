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
    /// A web service controller that may contain sub-controllers and/or a multiplexer.
    ///
    /// cache-control -- elimicates redundant requests (max-age) or data queries (not-modified).
    /// response cache -- directly returns shared cached contents
    /// etag -- reduces network I/O with unchanged results
    ///
    ///
    public abstract class WebService : WebRealm, IHttpApplication<HttpContext>
    {
        public WebServiceContext Context { get; internal set; }

        // topics published by this microservice
        readonly Set<MsgPublish> publishes;

        // topics subscribed by this microservice
        readonly Set<MsgSubscribe> subscribes;

        readonly KestrelServerOptions options;

        readonly LoggerFactory logger;

        // the embedded http server
        readonly KestrelServer server;

        private IPAddress mqaddr;
        private int mqport;

        // the async client
        private MsgClient client;


        protected WebService(WebServiceContext wsc) : base(wsc)
        {
            // init eqc client
            foreach (var ep in wsc.foreign)
            {
                //				ParseAddress()
            }

            // create the server instance
            logger = new LoggerFactory();

            options = new KestrelServerOptions();

            server = new KestrelServer(Options.Create(options), Lifetime, logger);
            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            addrs.Add(wsc.tls ? "https://" : "http://" + wsc.@public);
            addrs.Add("http://" + wsc.@internal); // clustered msg queue

            ParseAddress(wsc.@internal, out mqaddr, out mqport);
        }


        bool ParseAddress(string addr, out IPAddress ipaddr, out int port)
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

            if (port == mqport && ip.Equals(mqaddr))
            {
                // mq handling or action handling
                if (hc.Request.Path.Equals("*"))
                {
                    // msg queue
                }
                else
                {
                    // no auth
                    WebContext wc = (WebContext)hc;
                    Handle(hc.Request.Path.Value.Substring(1), wc);
                    await wc.Response.SendAsyncTask();
                }
                StringValues df = hc.Request.Headers["Range"];
            }
            else
            {
                WebContext wc = (WebContext)hc;
                Handle(hc.Request.Path.Value.Substring(1), wc);

                await wc.Response.SendAsyncTask();
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
        }


        public void Start()
        {
            // prepare event queue service
            //


            // start the server
            //
            server.Start(this);

            var urls = server.Features.Get<IServerAddressesFeature>().Addresses;

            Console.Write(Key);
            Console.Write(" =");
            foreach (var url in urls)
            {
                Console.Write(" ");
                Console.Write(url);
            }
            Console.WriteLine();
        }

        public void StopApplication()
        {
        }

        public CancellationToken ApplicationStarted { get; set; }

        public CancellationToken ApplicationStopping { get; set; }

        public CancellationToken ApplicationStopped { get; set; }


        public void Subscribe(string topic, MsgDoer doer)
        {
            subscribes.Add(new MsgSubscribe(topic, doer));
        }

        public SqlContext NewSqlContext()
        {
            WebService svc = Service;
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder()
            {
                Host = "localhost",
                Database = svc.Key,
                Username = "postgres",
                Password = "Zou###1989"
            };
            return new SqlContext(builder);
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