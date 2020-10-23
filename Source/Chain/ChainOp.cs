using System;
using System.Collections.Concurrent;
using System.Threading;
using SkyChain.Db;

namespace SkyChain.Chain
{
    public class ChainOp : DbOp
    {
        static readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        static Peer info;

        // a bundle of connected peers
        static readonly Map<string, ChainClient> clients = new Map<string, ChainClient>(32);

        // validate submitted entries and package demostic blocks 
        static Thread validator;

        // periodic polling of foreign blocks 
        static Thread poller;

        /// <summary>
        /// Setup blockchain on this peer node.
        /// </summary>
        public static void InitializeChain()
        {
            // ensure the related db structures
            if (!EnsureDb(true))
            {
                return;
            }

            // init connected peers
            LoadPeers();

            // init the poller thead
            if (clients != null)
            {
                // to repeatedly check and initiate event polling activities.
                poller = new Thread(Poll);
                poller.Start();
            }
        }

        static bool EnsureDb(bool create)
        {
            using var dc = NewDbContext();
            bool exists = dc.QueryTop("SELECT 1 FROM pg_namespace WHERE nspname = 'chain'");
            if (!exists && create)
            {
                // blocks table
                dc.Execute(@"
create table blocks (
    peerid varchar(8) not null,
    seq integer not null,
    stamp timestamp(0) not null,
    prevtag varchar(16) not null,
    tag varchar(16) not null,
    status smallint default 0 not null,
    constraint blocks_pk primary key (peerid, seq)
);"
                );

                dc.Execute(@"
create table blockrecs
(
    peerid varchar(8) not null,
    seq integer not null,
    acct varchar(30) not null,
    typ smallint not null,
    time timestamp(0) not null,
    oprid integer,
    descr varchar(20),
    amt money not null,
    bal money not null,
    doc jsonb,
    digest bigint,
    fpeerid varchar(8),
    constraint blockrecs_block_fk
        foreign key (peerid, seq) references blocks
);"
                );

                return true;
            }
            return exists;
        }

        static void LoadPeers()
        {
            @lock.EnterWriteLock();
            try
            {
                // clear up data maps
                clients.Clear();

                // load data types
                using var dc = NewDbContext();

                // load and init peer clients
                var arr = dc.Query<Peer>("SELECT * FROM chain.peers");
                if (arr == null) return;
                foreach (var o in arr)
                {
                    if (o.id == "&")
                    {
                        info = o;
                    }
                    else
                    {
                        var cli = new ChainClient(o);
                        clients.Add(cli);
                    }
                }
            }
            finally
            {
                @lock.ExitWriteLock();
            }
        }

        static void Poll(object state)
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

                        // start asynchronously
                        // var task = cli.TryPollAsync(tick);
                        // task.Start();
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

        static readonly Map<short, TransactDefinition> defs = new Map<short, TransactDefinition>(32);

        public static void Define(short typ, string name, params Activity[] steps)
        {
            var def = new TransactDefinition(typ, name, steps);
            defs.Add(def);
        }


        static readonly Queue queue = new Queue();
    }

    public class Queue : ConcurrentQueue<Operation>
    {
    }
}