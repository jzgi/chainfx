using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class Chain
    {
        //
        // source and caches
        //

        static DbSource dbsource;

        public static DbSource DbSource => dbsource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbsource == null) // check on-the-fly
            {
                throw new ApplicationException("missing 'chain' in json");
            }

            return dbsource.NewDbContext(level);
        }

        static List<DbCache> caches;

        static List<DbCache> objectcaches;

        static List<DbCache> mapcaches;

        public static void Cache<K, V>(Func<DbContext, Map<K, V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (caches == null)
            {
                caches = new List<DbCache>(16);
            }
            caches.Add(new DbCache<K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheObject<K, V>(Func<DbContext, K, V> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (objectcaches == null)
            {
                objectcaches = new List<DbCache>(16);
            }
            objectcaches.Add(new DbObjectCache<K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheMap<S, K, V>(Func<DbContext, S, Map<K, V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (mapcaches == null)
            {
                mapcaches = new List<DbCache>(8);
            }
            mapcaches.Add(new DbMapCache<S, K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheMap<S, K, V>(Func<DbContext, S, Task<Map<K, V>>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (mapcaches == null)
            {
                mapcaches = new List<DbCache>();
            }
            mapcaches.Add(new DbMapCache<S, K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static Map<K, V> Grab<K, V>(byte flag = 0) where K : IComparable<K>
        {
            if (caches == null)
            {
                return null;
            }
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbCache<K, V>) ca).Get();
                    }
                }
            }
            return null;
        }

        public static async Task<Map<K, V>> GrabAsync<K, V>(byte flag = 0) where K : IComparable<K>
        {
            if (caches == null)
            {
                return null;
            }
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return await ((DbCache<K, V>) ca).GetAsync();
                    }
                }
            }
            return null;
        }

        public static V GrabObject<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            if (objectcaches == null)
            {
                return default;
            }
            foreach (var ca in objectcaches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbObjectCache<K, V>) ca).Get(key);
                    }
                }
            }
            return default;
        }

        public static async Task<V> GrabObjectAsync<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            if (objectcaches == null)
            {
                return default;
            }
            foreach (var ca in objectcaches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return await ((DbObjectCache<K, V>) ca).GetAsync(key);
                    }
                }
            }
            return default;
        }


        public static Map<K, V> GrabMap<D, K, V>(D discr, byte flag = 0) where K : IComparable<K>
        {
            if (mapcaches == null)
            {
                return null;
            }
            foreach (var ca in mapcaches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbMapCache<D, K, V>) ca).Get(discr);
                    }
                }
            }
            return null;
        }

        public static async Task<Map<K, V>> GrabMapAsync<D, K, V>(D discr, byte flag = 0) where K : IComparable<K>
        {
            if (mapcaches == null)
            {
                return null;
            }
            foreach (var ca in mapcaches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return await ((DbMapCache<D, K, V>) ca).GetAsync(discr);
                    }
                }
            }
            return null;
        }

        //
        // inter-peer
        //


        static Peer info;

        // chainable table structures   
        static readonly Map<string, DbTable> tables = new Map<string, DbTable>(16);

        // connectors to remote peers 
        static readonly Map<short, ChainClient> clients = new Map<short, ChainClient>(16);


        internal static void InitializeChain(JObj chaincfg)
        {
            dbsource = new DbSource(chaincfg);

            // partial peer info
            info = new Peer(chaincfg);

            // scan chain tables
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'");
                var arr = dc.Query<DbTable>();
                if (arr != null)
                {
                    foreach (var o in arr)
                    {
                        if (!o.Key.EndsWith('_')) continue;

                        // select pg_get_serial_sequence('buys','id');

                        // dc.Sql("SELECT * FROM information_schema.sequences WHERE sequence_schema = 'public' AND sequence_name = ''");
                        // var conn = new DbColumn(null);
                        // o.Add(conn);

                        // dc.Sql("SELECT * FROM information_schema.columns WHERE table_schema = 'public' AND table_name = @1");

                        // init current block id
                        // await o.PeekLastBlockAsync(dc);
                    }
                }
            }


            // setup remote connectors
            //
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
                var arr = dc.Query<Peer>();
                if (arr != null)
                {
                    foreach (var o in arr)
                    {
                        var conn = new ChainClient(o);
                        clients.Add(conn);

                        // init current block id
                        // await o.PeekLastBlockAsync(dc);
                    }
                }
            }
        }


        public static Peer Info => info;

        //
        // transaction id generator

        static int lastId = 0;

        public static int AutoInc()
        {
            return Interlocked.Increment(ref lastId);
        }


        public static ChainClient GetClient(short peerid) => clients[peerid];

        public static Map<short, ChainClient> Clients => clients;

        public static ChainContext NewChainContext(IsolationLevel? level = null, short peerid = 0)
        {
            if (DbSource == null)
            {
                throw new ApplicationException("missing 'chain' in app.json");
            }

            return DbSource.NewChainContext(true, level);
        }


        //
        // instance scope
        //


        //


        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;

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
                            for (short k = 0; k < arr.Count; k++)
                            {
                                var o = arr[k];

                                // long seq = ChainUtility.WeaveSeq(blockid, i);
                                // dc.Sql("INSERT INTO chain.archive ").colset(Archival.Empty, extra: "peerid, cs, blockcs")._VALUES_(Archival.Empty, extra: "@1, @2, @3");
                                // direct parameter setting
                                var p = dc.ReCommand();
                                p.Digest = true;
                                // o.Write(p);
                                p.Digest = false;

                                // validate record-wise checksum
                                // if (o.cs != p.Checksum)
                                // {
                                //     cli.SetDataError(o.seq);
                                //     break;
                                // }

                                CryptoUtility.Digest(p.Checksum, ref bchk);
                                p.Set(p.Checksum); // set primary and checksum
                                // block-wise digest
                                if (i == 0) // begin of block 
                                {
                                    // if (o.blockcs != bchk) // compare with previous block
                                    // {
                                    //     cli.SetDataError("");
                                    //     break;
                                    // }

                                    p.Set(bchk);
                                }
                                else if (i == arr.Count - 1) // end of block
                                {
                                    // validate block check
                                    // if (bchk != o.blockcs)
                                    // {
                                    //     cli.SetDataError("");
                                    // }
                                    // p.Set(bchk = o.blockcs); // assign & set
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
        // chain ops
        //

        static readonly Map<string, ChainDuty> duties = new Map<string, ChainDuty>(8);

        public static void MakeDuty<D>(string name) where D : ChainDuty, new()
        {
            var dut = new D();
            duties.Add(name, dut);
        }

        public static ChainDuty GetDuty(string name) => duties[name];


        public static void Call(short peerid, string duty, string op)
        {
        }
    }
}