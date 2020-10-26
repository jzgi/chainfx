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
                using (var dc = NewDbContext())
                {
                    // typ cycle control
                    var now = DateTime.Now;
                    for (int i = 0; i < flows.Count; i++)
                    {
                        var def = flows.ValueAt(i);
                        var steplst = new ValueList<short>(def.Size);
                        foreach (var act in def.Activities)
                        {
                            if (act.IsNewCycle(now))
                            {
                                steplst.Add(act.step);
                            }
                        }
                        var vals = steplst.ToArray();
                        
                        var cc = new ChainContext();
                        
                        dc.Sql("SELECT * FROM chain.ops WHERE status = 2 AND typ = @1 AND step ")._IN_(vals);
                        dc.Query(p => p.Set(def.Typ).SetForIn(vals));
                        while (dc.Next())
                        {
                            var op = dc.ToObject<Operation>();
                            var act = def.GetActivity(op.step);
                            if (act == null)
                            {
                            }
                            // callback
                            var okay = act.OnValidate(cc);
                        }
                        
                    }
                }
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

        internal static readonly Map<short, ChainFlow> flows = new Map<short, ChainFlow>(32);

        public static void Define(short typ, string name, params ChainActivity[] steps)
        {
            var flow = new ChainFlow(
                typ,
                name,
                steps
            );
            flows.Add(flow);
        }

        public static ChainFlow GetFlow(short typ) => flows?[typ];
    }
}