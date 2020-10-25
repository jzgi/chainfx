using System;
using System.Threading;
using SkyChain.Db;

namespace SkyChain.Chain
{
    public class ChainEnv : DbEnv
    {
        static readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        static Peer info;

        // a bundle of connected peers
        static readonly Map<string, ChainClient> clients = new Map<string, ChainClient>(32);

        // validate submitted entries and package demostic blocks 
        static Thread validator;

        // periodic polling of foreign blocks 
        static Thread poller;

        public static Peer Info => info;

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

            // init the validator thead
            validator = new Thread(Validate);
            validator.Start();

            // init the poller thead
            if (clients.Count > 0)
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
);
                ");

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
                    if (o.IsMe)
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


        const int MAX_BLOCK_SIZE = 64;

        const int MAX_BLOCK_MINUTES = 24 * 60;

        static void Validate(object state)
        {
            while (true)
            {
                var lst = new ValueList<Operation>(MAX_BLOCK_SIZE);
                // archiving 
                using (var dc = NewDbContext())
                {
                    var c = lst.Count;
                    var last = lst[c - 1];
                    dc.Query("SELECT * FROM ops WHERE status = 2 AND tn > @1", p => p.Set(last.tn));
                    int num = 0;
                    while (dc.Next())
                    {
                        var op = dc.ToObject<Operation>();
                    }
                }

                // validating
                // Array.Sort(defs.All(), (a, b) => a.);
                // using (var dc = NewDbContext())
                // {
                // }
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

        internal static readonly Map<short, TransactDefinition> defs = new Map<short, TransactDefinition>(32);

        public static void Define(short typ, string name, params Activity[] steps)
        {
            var def = new TransactDefinition(typ, name, steps);
            defs.Add(def);
        }

        public static TransactDefinition GetDefinition(short typ) => defs?[typ];
    }
}