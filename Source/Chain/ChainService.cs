using System;
using System.Net;
using System.Threading;
using SkyCloud.Web;

namespace SkyCloud.Chain
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    [LoginAuthenticate]
    public class ChainService : WebService
    {
        readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        readonly Map<short, Typ> datyps = new Map<short, Typ>(16);

        // a bundle of peers
        readonly Map<string, ChainClient> clients = new Map<string, ChainClient>(32);

        // for identifying a peer by its IP address
        readonly Map<string, ChainClient> iptab = new Map<string, ChainClient>(64);

        // the thread schedules and drives periodic jobs, such as event polling 
        Thread scheduler;


        protected internal override void OnCreate()
        {
            // ensure DDL

            // load and setup peers
            Reload();

            // create and start the scheduler thead
            if (clients != null)
            {
                // to repeatedly check and initiate event polling activities.
                scheduler = new Thread(() =>
                {
                    while (true)
                    {
                        // interval
                        Thread.Sleep(7000);

                        // a scheduling cycle
                        var tick = Environment.TickCount;
                        @lock.EnterReadLock();
                        try
                        {
                            var clis = clients;
                            for (int i = 0; i < clients.Count; i++)
                            {
                                var cli = clis.ValueAt(i);
                                cli.TryPollAsync(tick);
                            }
                        }
                        finally
                        {
                            @lock.ExitReadLock();
                        }
                    }
                });
                scheduler.Start();
            }

            CreateWork<AdmlyWork>("admly");
        }

        void Reload()
        {
            @lock.EnterWriteLock();
            try
            {
                // clear up data maps
                datyps.Clear();
                clients.Clear();
                iptab.Clear();

                // load data types
                using var dc = NewDbContext();
                dc.Sql("SELECT * FROM chain.datyps");
                while (dc.Next())
                {
                    var dt = dc.ToObject<Typ>();
                    datyps.Add(dt);
                }

                // load and init peer clients
                var arr = dc.Query<Peer>("SELECT * FROM chain.peers");
                foreach (var o in arr)
                {
                    if (o.id == "&") continue;
                    var cli = new ChainClient(o.raddr);
                    clients.Add(cli);
                    // add to iptab 
                    var addrs = Dns.GetHostAddresses(o.raddr);
                    foreach (var a in addrs)
                    {
                        iptab.Add(a.ToString(), cli);
                    }
                }
            }
            finally
            {
                @lock.ExitWriteLock();
            }
        }


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