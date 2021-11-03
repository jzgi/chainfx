using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Chain
{
    public class Chain
    {
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

        //
        // db-object cache
        //

        static List<DbCache> maps;

        static List<DbCache> valuesets;

        static List<DbCache> mapsets;

        public static void CacheMap<K, V>(Func<DbContext, Map<K, V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (maps == null)
            {
                maps = new List<DbCache>(16);
            }
            maps.Add(new DbMap<K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void Cache<K, V>(Func<DbContext, K, V> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (valuesets == null)
            {
                valuesets = new List<DbCache>(16);
            }
            valuesets.Add(new DbValueSet<K, V>(fetcher, typeof(V), maxage, flag));
        }

        // public static void CacheValue<K, V>(Func<DbContext, K, Task<V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        // {
        //     if (valuecolls == null)
        //     {
        //         valuecolls = new List<DbCache>(16);
        //     }
        //     valuecolls.Add(new DbValueCollection<K, V>(fetcher, typeof(V), maxage, flag));
        // }
        //

        public static void CacheSub<S, K, V>(Func<DbContext, S, Map<K, V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (mapsets == null)
            {
                mapsets = new List<DbCache>();
            }
            mapsets.Add(new DbMapSet<S, K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheSub<S, K, V>(Func<DbContext, S, Task<Map<K, V>>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (mapsets == null)
            {
                mapsets = new List<DbCache>();
            }
            mapsets.Add(new DbMapSet<S, K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static Map<K, V> ObtainMap<K, V>(byte flag = 0) where K : IComparable<K>
        {
            if (maps == null)
            {
                return null;
            }
            foreach (var cache in maps)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return ((DbMap<K, V>) cache).Get();
                    }
                }
            }
            return null;
        }

        public static async Task<Map<K, V>> ObtainMapAsync<K, V>(byte flag = 0) where K : IComparable<K>
        {
            if (maps == null)
            {
                return null;
            }
            foreach (var cache in maps)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return await ((DbMap<K, V>) cache).GetAsync();
                    }
                }
            }
            return null;
        }

        public static V Obtain<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            if (valuesets == null)
            {
                return default;
            }
            foreach (var cache in valuesets)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return ((DbValueSet<K, V>) cache).Get(key);
                    }
                }
            }
            return default;
        }

        public static async Task<V> ObtainAsync<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            if (valuesets == null)
            {
                return default;
            }
            foreach (var cache in valuesets)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return await ((DbValueSet<K, V>) cache).GetAsync(key);
                    }
                }
            }
            return default;
        }


        public static Map<K, V> ObtainSub<D, K, V>(D discr, byte flag = 0) where K : IComparable<K>
        {
            if (mapsets == null)
            {
                return null;
            }
            foreach (var cache in mapsets)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return ((DbMapSet<D, K, V>) cache).Get(discr);
                    }
                }
            }
            return null;
        }

        public static async Task<Map<K, V>> ObtainSubAsync<D, K, V>(D discr, byte flag = 0) where K : IComparable<K>
        {
            if (mapsets == null)
            {
                return null;
            }
            foreach (var cache in mapsets)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return await ((DbMapSet<D, K, V>) cache).GetAsync(discr);
                    }
                }
            }
            return null;
        }


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

        public static ChainDrive Drive { get; protected set; }

        //
        // transaction id generator

        static int lastId = 0;

        public static int AutoInc()
        {
            return Interlocked.Increment(ref lastId);
        }


        public static ChainClient GetClient(short peerid) => clients[peerid];

        public static Map<short, ChainClient> Clients => clients;

        public static ChainContext NewChainContext(IsolationLevel? level = null)
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

        // declared operations 
        readonly Map<string, ChainAction> ops = new Map<string, ChainAction>(32);


        public ChainAction GetAction(string name) => ops[name];


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
        // SUB CONTEXT
        //

        static readonly ConcurrentDictionary<long, ChainContext> slaves = new ConcurrentDictionary<long, ChainContext>();


        internal static ChainContext AcquireSlave(long id, IsolationLevel level)
        {
            if (id > 0)
            {
                var ctx = slaves[id];
                return ctx;
            }
            else
            {
                var ctx = NewChainContext(level);
                slaves.TryAdd(id, ctx);
                return ctx;
            }
        }

        static void Clean()
        {
        }
    }
}