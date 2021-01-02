using System;
using System.Data;
using System.Threading;
using SkyChain.Db;

namespace SkyChain.Chain
{
    public class ChainEnviron : DbEnviron
    {
        static readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        static Peer info;

        // a bundle of connected peers
        static readonly Map<string, ChainClient> clients = new Map<string, ChainClient>(32);

        // validates logs and archives demostic blocks 
        static Thread archiver;

        // periodic polling of foreign blocks 
        static Thread poller;

        public static Peer Info => info;

        public static ChainClient GetChainClient(string key) => clients[key];

        /// <summary>
        /// Sets up and start blockchain on this peer node.
        /// </summary>
        public static void StartChain()
        {
            // ensure the related db structures
            if (!EnsureDb(true))
            {
                return;
            }

            // init connected peers
            InitializePeers();

            // init the validator thead
            archiver = new Thread(Archive);
            archiver.Start();

            // init the poller thead
            // if (clients.Count > 0)
            // {
            //     // to repeatedly check and initiate event polling activities.
            //     poller = new Thread(Poll);
            //     // poller.Start();
            // }
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

        static void InitializePeers()
        {
            @lock.EnterWriteLock();
            try
            {
                // clear up data maps
                clients.Clear();

                // load data types
                using var dc = NewDbContext();

                // load and init peer clients
                dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
                var arr = dc.Query<Peer>();
                if (arr == null) return;
                foreach (var o in arr)
                {
                    if (o.IsLocal)
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

        const int MIN_BLOCK_SIZE = 8;

        static void Archive(object state)
        {
            while (true)
            {
                Thread.Sleep(60 * 1000); // 60 seconds interval

                Cycle:

                // archiving 
                using (var dc = NewDbContext(IsolationLevel.ReadCommitted))
                {
                    var lst = new ValueList<Log>(MAX_BLOCK_SIZE);
                    dc.Sql("SELECT ").collst(Log.Empty).T(" FROM chain.logs WHERE status = ").T(Log.DONE).T(" ORDER BY stamp LIMIT ").T(MAX_BLOCK_SIZE);
                    dc.Query();

                    int dgst = 0; // total digest of the list
                    while (dc.Next())
                    {
                        var o = dc.ToObject<Log>(State.DIGEST);
                        lst.Add(o);
                        CryptionUtility.Digest(o.dgst, ref dgst);
                    }
                    if (lst.Count < MIN_BLOCK_SIZE)
                    {
                        continue;
                    }

                    // insert

                    dc.Sql("INSERT INTO chain.blocks (peerid, stamp, status, dgst, pdgst) VALUES (@1, @2, 0, @3, (SELECT dgst FROM chain.blocks ORDER BY peerid, seq LIMIT 1)) RETURNING seq");
                    dc.Query(p => p.Set(info.id).Set(DateTime.Now).Set(dgst));
                    dc.Let(out int seq);
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var o = lst[i];
                        dc.Sql("INSERT INTO chain.blocksts ").colset(State.Empty, 0, "peerid, seq")._VALUES_(State.Empty, 0, "@1, @2");
                        dc.Execute(p =>
                        {
                            o.Write(p, 0);
                            p.Set(info.id).Set(seq);
                        });
                    }

                    // delete
                    var sql = dc.Sql("DELETE FROM chain.logs WHERE  status = ").T(Log.DONE).T(" AND (job, step) IN (");
                    for (int i = 0; i < lst.Count; i++)
                    {
                        var o = lst[i];
                        if (i > 0)
                        {
                            sql.T(',');
                        }
                        sql.T('(').T('\'').T(o.job).T('\'').T(',').T(o.step).T(')');
                    }
                    sql.T(")");
                }

                goto Cycle;
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
    }
}