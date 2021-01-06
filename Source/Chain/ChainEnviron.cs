using System;
using System.Data;
using System.Threading;
using SkyChain.Db;

namespace SkyChain.Chain
{
    public class ChainEnviron : DbEnviron
    {
        static Peer info;

        // a bundle of connected peers
        static readonly Map<short, ChainClient> clients = new Map<short, ChainClient>(32);

        // validates ops and archives demostic blocks 
        static Thread archiver;

        // periodic polling of foreign blocks 
        static Thread poller;

        public static Peer Info => info;

        public static ChainClient GetChainClient(short key) => clients[key];

        /// <summary>
        /// Sets up and start blockchain on this peer node.
        /// </summary>
        public static void StartChain()
        {
            // all connected peers
            InitializePeers();

            // the archiver thead
            archiver = new Thread(Archive);
            archiver.Start();

            // the poller thead
            if (clients.Count > 0)
            {
                poller = new Thread(Poll);
                // poller.Start();
            }
        }


        static void InitializePeers()
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


        const int MAX_BLOCK_SIZE = 32;

        const int MIN_BLOCK_SIZE = 8;

        static async void Archive(object state)
        {
            int seq = 0;
            long lastdgst = 0;
            while (true)
            {
                Thread.Sleep(60 * 1000); // 60 seconds interval

                Cycle:

                // archiving 
                using (var dc = NewDbContext(IsolationLevel.ReadCommitted))
                {
                    dc.Sql("SELECT ").collst(BlockOp.Empty).T(" FROM chain.ops WHERE status = ").T(Op.ENDED).T(" ORDER BY stamp LIMIT ").T(MAX_BLOCK_SIZE);
                    var arr = await dc.QueryAsync<BlockOp>();
                    if (arr == null || arr.Length < MIN_BLOCK_SIZE)
                    {
                        continue;
                    }

                    // insert archives
                    if (seq == 0 && dc.QueryTop("SELECT seq, dgst FROM chain.blocks ORDER BY peerid, seq LIMIT 1"))
                    {
                        dc.Let(out seq); // last seq
                        dc.Let(out lastdgst);
                    }
                    seq++;
                    var blk = new Block
                    {
                        peerid = info.id,
                        seq = seq,
                        stamp = DateTime.Now,
                        status = 0,
                        pdgst = lastdgst,
                        dgst = 0, // calculated later
                    };
                    dc.Sql("INSERT INTO ").collst(Block.Empty)._VALUES_(Block.Empty);
                    await dc.QueryAsync(p => blk.Write(p));
                    long totalcs = 0; // total checksum
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var o = arr[i];
                        dc.Sql("INSERT INTO chain.blockrcs ").colset(BlockOp.Empty, 0, "peerid, seq, dgst")._VALUES_(BlockOp.Empty, 0, "@1, @2, @3");
                        await dc.ExecuteAsync(p =>
                        {
                            p.Digest = true;
                            o.Write(p, 0);
                            p.Digest = false;
                            p.Set(info.id).Set(seq).Set(p.Checksum);

                            CryptionUtility.Digest(p.Checksum, ref totalcs);
                        });
                    }
                    await dc.ExecuteAsync("UPDATE chain.blocks SET dgst = @1 WHERE peerid = @1 AND seq = @2", p => p.Set(blk.peerid).Set(totalcs));
                    // keep digest used in next cycle
                    lastdgst = totalcs;

                    // delete ops
                    var s = dc.Sql("DELETE FROM chain.ops WHERE status = ").T(Op.ENDED).T(" AND (job, step) IN (");
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var o = arr[i];
                        if (i > 0) s.T(',');
                        s.T('(').T(o.job).T(',').T(o.step).T(')');
                    }
                    s.T(')');
                    await dc.ExecuteAsync(prepare: false);
                }
                goto Cycle;
            }
        }

        static async void Poll(object state)
        {
            while (true)
            {
                Thread.Sleep(60 * 1000); // 60 seconds interval

                Cycle:
                // a scheduling cycle
                var tick = Environment.TickCount;

                int busy = 0;
                for (int i = 0; i < clients.Count; i++)
                {
                    var cli = clients.ValueAt(i);

                    if (cli.IsCompleted)
                    {
                        var (block, states) = cli.Result;

                        // db
                    }

                    if (busy == 0)
                    {
                        goto Cycle;
                    }
                }
            }
        }
    }
}