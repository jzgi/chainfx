using System;
using System.Net;
using System.Threading;
using CloudUn.Web;

namespace CloudUn.Net
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    [UserAuthenticate]
    public class ChainService : WebService
    {
        // a bundle of peers
        Map<string, ChainPeer> peers = null;

        // for identifying a peer by its IP address
        Map<IPAddress, ChainPeer> lookup = null;

        // the thread schedules and drives periodic jobs, such as event polling 
        Thread scheduler;


        protected internal override void OnCreate()
        {
            // ensure DDL

            // load and setup peers
            using var dc = NewDbContext();
            dc.Query("SELECT * FROM chain.peers WHERE status > 0");
            while (dc.Next())
            {
                var p = dc.ToObject<Peer>();

                var cp = new ChainPeer("");

                var addrs = Dns.GetHostAddresses("");

                peers.Add("", cp);
                foreach (var addr in addrs)
                {
                    lookup.Add(addr, cp);
                }
            }

            // create and start the scheduler thead
            if (peers != null)
            {
                // to repeatedly check and initiate event polling activities.
                scheduler = new Thread(() =>
                {
                    while (true)
                    {
                        // interval
                        Thread.Sleep(1000);

                        // a schedule cycle
                        int tick = Environment.TickCount;
                        for (int i = 0; i < peers.Count; i++)
                        {
                            var cli = peers.ValueAt(i);
                            cli.TryPollAsync(tick);
                        }
                    }
                });
                scheduler.Start();
            }

            CreateWork<AdmlyWork>("admly");
        }
        // inter-node


        // HTTP Data API
        public void token(WebContext wc)
        {
            string id = wc.Query[nameof(id)];
            string password = wc.Query[nameof(password)];

            using var dc = NewDbContext();
            
            // retrieve from idents
        }

        public void query(WebContext wc)
        {
        }

        public void querya(WebContext wc)
        {
        }

        public void put(WebContext wc)
        {
        }
    }
}