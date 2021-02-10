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

        // periodic polling and importing of foreign blocks 
        static Thread importer;

        public static Peer Info => info;

        public static ChainClient GetChainClient(short peerid) => clients[peerid];

        public static Map<short, ChainClient> Clients => clients;

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
                importer = new Thread(Import);
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
            long blockcs = 0;
            while (true)
            {
                Thread.Sleep(90 * 1000); // 90 seconds interval

                // archiving cycle 
                Cycle:
                using (var dc = NewDbContext(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        dc.Sql("SELECT ").collst(FlowState.Empty).T(" FROM chain.ops WHERE status = ").T(FlowOp.STATUS_ENDED).T(" ORDER BY stamp LIMIT ").T(MAX_BLOCK_SIZE);
                        var arr = await dc.QueryAsync<FlowState>();
                        if (arr == null || arr.Length < MIN_BLOCK_SIZE)
                        {
                            continue; // go for delay
                        }

                        // resolve new block id
                        if (blockid == 0 && await dc.QueryTopAsync("SELECT seq, blockcs FROM chain.blocks WHERE peerid = @1 ORDER BY seq DESC LIMIT 1"))
                        {
                            dc.Let(out long seq); // last seq
                            dc.Let(out blockcs);
                            var (bid, _) = ChainUtility.ResolveSeq(seq);
                            blockid = bid;
                        }
                        blockid++;

                        // insert archivals
                        //
                        long bchk = 0; // current block checksum
                        dc.Sql("INSERT INTO chain.blocks ").colset(FlowState.Empty, extra: "peerid, seq, cs, blockcs")._VALUES_(FlowState.Empty, extra: "@1, @2, @3, @4");
                        for (short i = 0; i < arr.Length; i++)
                        {
                            var o = arr[i];
                            // set parameters
                            var p = dc.ReCommand();
                            p.Digest = true;
                            o.Write(p);
                            p.Digest = false;
                            CryptoUtility.Digest(p.Checksum, ref bchk);
                            p.Set(info.id).Set(ChainUtility.WeaveSeq(blockid, i)).Set(p.Checksum); // set primary and checksum
                            // set block-wise digest
                            if (i == 0) // begin of block 
                            {
                                p.Set(blockcs);
                            }
                            else if (i == arr.Length - 1) // end of block
                            {
                                p.Set(blockcs = bchk); // assign & set
                            }
                            else
                            {
                                p.SetNull();
                            }
                            await dc.SimpleExecuteAsync();
                        }

                        // delete operationals
                        //
                        dc.Sql("DELETE FROM chain.ops WHERE status = ").T(FlowOp.STATUS_ENDED).T(" AND job = @1 AND step = @2");
                        for (int i = 0; i < arr.Length; i++)
                        {
                            var o = arr[i];
                            await dc.ExecuteAsync(p => p.Set(o.job).Set(o.step));
                        }
                    }
                    catch (Exception e)
                    {
                        dc.Rollback();
                        Framework.ERR(e.Message);
                        return; // quit the loop
                    }
                }
                goto Cycle;
            }
        }

        static async void Import(object state)
        {
            while (true)
            {
                Cycle:
                var outer = true;
                Thread.Sleep(60 * 1000); // 60 seconds delay
                for (int i = 0; i < clients.Count; i++) // LOOP
                {
                    int busy = 0;
                    var cli = clients.ValueAt(i);
                    var code = cli.TryReap(out var arr);
                    if (code == 200)
                    {
                        try
                        {
                            using var dc = NewDbContext(IsolationLevel.ReadCommitted);
                            long bchk = 0; // block checksum
                            for (short k = 0; k < arr.Length; k++)
                            {
                                var o = arr[k];

                                // long seq = ChainUtility.WeaveSeq(blockid, i);
                                dc.Sql("INSERT INTO chain.blocks ").colset(Archival.Empty, extra: "peerid, cs, blockcs")._VALUES_(Archival.Empty, extra: "@1, @2, @3");
                                // direct parameter setting
                                var p = dc.ReCommand();
                                p.Digest = true;
                                o.Write(p);
                                p.Digest = false;

                                // validate record-wise checksum
                                if (o.cs != p.Checksum)
                                {
                                    cli.SetDataError(o.seq);
                                    break;
                                }

                                CryptoUtility.Digest(p.Checksum, ref bchk);
                                p.Set(p.Checksum); // set primary and checksum
                                // block-wise digest
                                if (i == 0) // begin of block 
                                {
                                    if (o.blockcs != bchk) // compare with previous block
                                    {
                                        cli.SetDataError("");
                                        break;
                                    }

                                    p.Set(bchk);
                                }
                                else if (i == arr.Length - 1) // end of block
                                {
                                    // validate block check
                                    if (bchk != o.blockcs)
                                    {
                                        cli.SetDataError("");
                                    }
                                    p.Set(bchk = o.blockcs); // assign & set
                                }
                                else
                                {
                                    p.SetNull();
                                }

                                await dc.SimpleExecuteAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            cli.SetInternalError(null);
                        }
                    }

                    if (code == 200 || (outer && (code == 0 || code == 204)))
                    {
                        cli.ScheduleRemotePoll(0);
                        busy++;
                    }

                    //
                    if (busy == 0) // to outer for delay
                    {
                        break;
                    }
                    outer = false;
                }
            } // outer
        }
    }
}