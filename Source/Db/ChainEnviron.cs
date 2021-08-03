using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainEnviron : DbEnviron
    {
        static Peer info;

        // connectors to remote peers 
        static readonly Map<short, ChainClient> clients = new Map<short, ChainClient>(32);

        // polls and imports foreign blocks 
        static Thread importer;

        // logics that determine in-peer and inter-peer transactions' behaviors  
        static readonly Map<short, ChainLogic> logics = new Map<short, ChainLogic>(32);


        public static ChainLogic GetLogic(short typ) => logics[typ];

        public static Peer Info
        {
            get => info;
            internal set => info = value;
        }


        //
        // transaction id generator

        static int lastId = 0;

        static int generateId()
        {
            return Interlocked.Increment(ref lastId);
        }


        public static ChainClient GetClient(short peerid) => clients[peerid];


        public static Map<short, ChainClient> Clients => clients;

        public static ChainContext NewChainContext(IsolationLevel? level = null)
        {
            if (dbsource == null)
            {
                throw new ServerException("missing 'db' in app.json");
            }

            return dbsource.NewChainContext(true, level);
        }


        /// <summary>
        /// Creates a chain logic that demetermines behaviors for transactions.
        /// </summary>
        /// <param name="txtyp"></param>
        /// <typeparam name="L"></typeparam>
        public static void MakeLogic<L>(short txtyp) where L : ChainLogic, new()
        {
            var v = new L
            {
            };
            logics.Add(v);
        }


        /// <summary>
        /// Sets up and start blockchain on this peer node.
        /// </summary>
        public static async Task StartChainAsync()
        {
            // clear up data maps
            clients.Clear();

            //load this node peer
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers WHERE native = TRUE");
                info = await dc.QueryTopAsync<Peer>();
                // get current blockid
                if (info != null)
                {
                    await info.PeekLastBlockAsync(dc);
                }
            }

            // load remote connectors
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers WHERE native = FALSE");
                var arr = await dc.QueryAsync<Peer>();
                if (arr != null)
                {
                    foreach (var o in arr)
                    {
                        var conn = new ChainClient(o);
                        clients.Add(conn);

                        // init current block id
                        await o.PeekLastBlockAsync(dc);
                    }
                }
            }

            // start the importer thead
            importer = new Thread(Import);
            importer.Start();
        }


        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;

        static async void Commit(object state)
        {
            while (info != null && info.IsRunning)
            {
                Thread.Sleep(60 * 1000);

                // archiving journal entries 
                Cycle:
                using (var dc = NewDbContext(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        dc.Sql("SELECT ").collst(OperativeRow.Empty).T(" FROM chain.queue ORDER BY id LIMIT ").T(MAX_BLOCK_SIZE);
                        var arr = await dc.QueryAsync<OperativeRow>();
                        if (arr == null || arr.Length < MIN_BLOCK_SIZE)
                        {
                            continue; // go for delay
                        }

                        // resolve last block
                        if (info.blockid == 0)
                        {
                            await info.PeekLastBlockAsync(dc);
                        }
                        info.IncrementBlockId();

                        // insert archivals
                        //
                        long bchk = 0; // current block checksum
                        dc.Sql("INSERT INTO chain.archive ").colset(_Ety.Empty, extra: "peerid, seq, cs, blockcs")._VALUES_(_Ety.Empty, extra: "@1, @2, @3, @4");
                        for (short i = 0; i < arr.Length; i++)
                        {
                            var o = arr[i];
                            // set parameters
                            var p = dc.ReCommand();
                            p.Digest = true;
                            o.Write(p);
                            p.Digest = false;
                            CryptoUtility.Digest(p.Checksum, ref bchk);
                            p.Set(info.id).Set(ChainUtility.WeaveSeq(info.blockid, i)).Set(p.Checksum); // set primary and checksum
                            // set block-wise digest
                            if (i == 0) // begin of block 
                            {
                                p.Set(info.blockcs);
                            }
                            else if (i == arr.Length - 1) // end of block
                            {
                                p.Set(info.blockcs = bchk); // assign & set
                            }
                            else
                            {
                                p.SetNull();
                            }
                            await dc.SimpleExecuteAsync();
                        }

                        // remove from queue
                        //
                        var lastid = arr[^1].id;
                        dc.Sql("DELETE FROM chain.queue WHERE id <= @1");
                        await dc.ExecuteAsync(p => p.Set(lastid));
                    }
                    catch (Exception e)
                    {
                        dc.Rollback();
                        ServerEnviron.ERR(e.Message);
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

                Thread.Sleep(60 * 1000);

                for (int i = 0; i < clients.Count; i++) // LOOP
                {
                    var cli = clients.ValueAt(i);

                    if (!cli.Info.IsRunning) continue;

                    int busy = 0;
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
                                dc.Sql("INSERT INTO chain.archive ").colset(Archival.Empty, extra: "peerid, cs, blockcs")._VALUES_(Archival.Empty, extra: "@1, @2, @3");
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
                            cli.SetInternalError(e.Message);
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


        //
        // SUB CONTEXT
        //

        static readonly ConcurrentDictionary<long, ChainContext> slavectxs = new ConcurrentDictionary<long, ChainContext>();


        internal static ChainContext AcquireSlave(long id, IsolationLevel level)
        {
            if (id > 0)
            {
                var ctx = slavectxs[id];
                return ctx;
            }
            else
            {
                var ctx = NewChainContext(level);
                slavectxs.TryAdd(id, ctx);
                return ctx;
            }
        }

        static void Clean()
        {
        }
    }
}