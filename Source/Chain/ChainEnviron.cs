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

        public static ChainClient GetChainClient(short peerid) => clients[peerid];

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

            // load and init peer clients
            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
            var arr = dc.Query<Peer>();
            if (arr == null) return;
            foreach (var o in arr)
            {
                if (o.Local)
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


        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;

        static async void Archive(object state)
        {
            int blockid = 0;
            long blockchk = 0;
            while (true)
            {
                Thread.Sleep(60 * 1000); // 60 seconds interval

                Cycle:

                // archiving 
                using (var dc = NewDbContext(IsolationLevel.ReadCommitted))
                {
                    dc.Sql("SELECT ").collst(Arch.Empty).T(" FROM chain.ops WHERE status = ").T(Op.ENDED).T(" ORDER BY stamp LIMIT ").T(MAX_BLOCK_SIZE);
                    var arr = await dc.QueryAsync<Arch>();
                    if (arr == null || arr.Length < MIN_BLOCK_SIZE)
                    {
                        continue;
                    }

                    // insert archivals
                    if (blockid == 0 && await dc.QueryTopAsync("SELECT seq, blockchk FROM chain.blocks WHERE peerid = @1 ORDER BY seq DESC LIMIT 1"))
                    {
                        dc.Let(out long seq); // last seq
                        dc.Let(out blockchk);
                        var (bid, _) = ChainUtility.ResolveSeq(seq);
                        blockid = bid;
                    }
                    blockid++;

                    long chk = 0; // block checksum
                    for (short i = 0; i < arr.Length; i++)
                    {
                        var o = arr[i];
                        long seq = ChainUtility.WeaveSeq(blockid, i);
                        dc.Sql("INSERT INTO chain.blocks ").colset(Arch.Empty, extra: "peerid, seq, chk, blockchk")._VALUES_(Arch.Empty, extra: "@1, @2. @3, @4");
                        await dc.ExecuteAsync(p =>
                        {
                            p.Digest = true;
                            o.Write(p);
                            p.Digest = false;
                            CryptionUtility.Digest(p.Checksum, ref chk);
                            p.Set(info.id).Set(seq).Set(p.Checksum); // set primary and checksum
                            // block-wise digest
                            if (i == 0) // begin of block 
                            {
                                p.Set(blockchk);
                            }
                            else if (i == arr.Length - 1) // end of block
                            {
                                p.Set(blockchk = chk); // assign & set
                            }
                            else
                            {
                                p.SetNull();
                            }
                        });
                    }

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
                Cycle:
                Thread.Sleep(60 * 1000); // 60 seconds depay
                var fresh = true; // a new cycle after the depay
                for (int i = 0; i < clients.Count; i++) // LOOP
                {
                    int busy = 0;
                    var cli = clients.ValueAt(i);
                    var code = cli.TryReap(out var arr);
                    if (code == 200)
                    {
                        using var dc = NewDbContext(IsolationLevel.ReadCommitted);
                        long chk = 0; // block checksum
                        for (short k = 0; k < arr.Length; k++)
                        {
                            var o = arr[k];
                            // long seq = ChainUtility.WeaveSeq(blockid, i);
                            dc.Sql("INSERT INTO chain.blocks ").colset(Arch.Empty, Arch.INSTALL, extra: "chk, blockchk")._VALUES_(Arch.Empty, Arch.INSTALL, extra: "@1, @2");
                            await dc.ExecuteAsync(p =>
                            {
                                p.Digest = true;
                                o.Write(p);
                                p.Digest = false;

                                // validate row check
                                if (o.chk != p.Checksum)
                                {
                                }

                                CryptionUtility.Digest(p.Checksum, ref chk);
                                p.Set(p.Checksum); // set primary and checksum
                                // block-wise digest
                                if (i == 0) // begin of block 
                                {
                                    // p.Set(blockchk);
                                }
                                else if (i == arr.Length - 1) // end of block
                                {
                                    // validate block check
                                    if (chk != o.blockchk)
                                    {
                                    }
                                    // p.Set(blockchk = chk); // assign & set
                                }
                                else
                                {
                                    p.SetNull();
                                }
                            });
                        }
                    }

                    if (code == 200 || (fresh && (code == 0 || code == 204)))
                    {
                        cli.StartRemote(0);
                        busy++;
                    }

                    //
                    if (busy == 0) // to delay
                    {
                        goto Cycle;
                    }
                    fresh = false;
                } // LOOP
            } // CYCLE
        }
    }
}