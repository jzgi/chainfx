using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ChainFx.Web
{
    /// <summary>
    /// A web content proxy or/and web event broker.
    /// </summary>
    public sealed class WebProxy : WebService
    {
        //
        // the forwarding support
        //

        // uri to the origin service
        string forward;

        // client connector to the origin service
        WebClient connector;

        //
        // accumulate events and the synchronization cycle
        //

        // in seconds, interval for web event processing cycle
        internal short cycle;

        readonly TextContent loads = new TextContent(true, 1024 * 256);

        private Thread cycler;

        internal override void Initialize(string prop, JObj webcfg)
        {
            base.Initialize(prop, webcfg);

            // forward url
            forward = webcfg[nameof(forward)];
            if (forward != null)
            {
                connector = new WebClient(forward);
            }

            // cycle internal
            cycle = webcfg[nameof(cycle)];
            if (forward != null && cycle > 0)
            {
            }
        }

        public string Forward => forward;

        public short Cycle => cycle;


        protected internal override async Task StartAsync(CancellationToken token)
        {
            await base.StartAsync(token);

            // create & start the poller thread
            if (cycle > 0)
            {
                cycler = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // polling cycle
                        Thread.Sleep(1000 * cycle);

                        // loop to clear or remove each expired items
                        int now = Environment.TickCount;
                        // client.GetArrayAsync<>()
                    }
                });
                cycler.Start();
            }

            if (forward != null)
            {
                Console.WriteLine("as a proxy to [" + forward + "]" + Url);
            }
        }

        public override Task ProcessRequestAsync(HttpContext context)
        {
            var wc = (WebContext) context;
            return base.ProcessRequestAsync(context);
        }
    }
}