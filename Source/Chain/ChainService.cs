using System;
using System.Net;
using System.Threading;
using Skyiah.Web;

namespace Skyiah.Chain
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    public class ChainService : WebService
    {
        readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        // a bundle of peers
        readonly Map<string, WebConnect> clients = new Map<string, WebConnect>(32);

        // for identifying a peer by its IP address
        readonly Map<string, WebConnect> iptab = new Map<string, WebConnect>(64);

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
        }

        void Reload()
        {
            @lock.EnterWriteLock();
            try
            {
                // clear up data maps
                clients.Clear();
                iptab.Clear();

                // load data types
                using var dc = NewDbContext();

                // load and init peer clients
                var arr = dc.Query<Peer>("SELECT * FROM chain.peers");
                if (arr == null) return;
                foreach (var o in arr)
                {
                    if (o.id == "&") continue;
                    var cli = new WebConnect(o.raddr);
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


        [Get("Get access token for a login", query: "?id=<-login-id->&password=")]
        [Reply(200, "Success", body: @"{
            name : string, // login name
            id : string, // login id
            token : // access token
        }")]
        [Reply(404, "login not found or invalid password")]
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