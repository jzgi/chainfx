using System;
using System.Net;
using System.Threading;
using SkyChain.Db;

namespace SkyChain.Chain
{
    public class ChainAccess : DbAccess
    {
        static readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        // a bundle of peers
        static readonly Map<string, ChainConnect> clients = new Map<string, ChainConnect>(32);

        // for identifying a peer by its IP address
        static readonly Map<string, ChainConnect> iptab = new Map<string, ChainConnect>(64);

        // the thread schedules and drives periodic jobs, such as event polling 
        static Thread scheduler;

        internal static void ConfigureChain(bool ddl)
        {
            // ensure DDL
            using var dc = NewDbContext();
            if (!dc.QueryTop("SELECT 1 FROM pg_namespace WHERE nspname = 'chain'"))
            {
                if (ddl) // create tables
                {
                }
                else
                {
                    return;
                }
            }

            // load and setup peers
            Reload();

            // create and start the scheduler thead
            if (clients != null)
            {
                // to repeatedly check and initiate event polling activities.
                scheduler = new Thread(Orchestrate);
                scheduler.Start();
            }
        }

        static void Reload()
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
                var arr = dc.Query<Peer>("SELECT * FROM chain.nodes");
                if (arr == null) return;
                foreach (var o in arr)
                {
                    if (o.id == "&") continue;
                    var cli = new ChainConnect(o.raddr);
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

        static void Orchestrate(object state)
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
        }

        //
        // types
        //

        static readonly Map<short, TransactDescriptor> descrs = new Map<short, TransactDescriptor>(32);

        public static void DefineTransact<T, C>(short typ, string name) where T : Transit, new() where C : Consent, new()
        {
            var descr = new TransactDescriptor(
                typ,
                name,
                new T(),
                new C()
            );
            descrs.Add(descr);
        }
    }
}