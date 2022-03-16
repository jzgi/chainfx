using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Nodal
{
    /// <summary>
    /// The static home environment for data store and being a node of the federated blockchain network. 
    /// </summary>
    public abstract class Home
    {
        // db
        //

        static DbSource dbSource;

        static List<DbCache> caches; // an entire map (standard)

        static List<DbCache> objectCaches; // once an object a time

        static List<DbCache> mapCaches; // many a map


        // connectivty
        //

        static Peer self;

        static readonly Map<short, NodeClient> connectors = new Map<short, NodeClient>(32);

        static bool hub;


        // imports foreign blocks 
        //

        static Thread replicator;


        internal static void InitializeHome(JObj homecfg)
        {
            // create db source
            dbSource = new DbSource(homecfg);

            // create self peer info
            self = new Peer(homecfg)
            {
            };

            // load & setup peer connectors
            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM peers_ WHERE status >= 0");
            var arr = dc.Query<Peer>();
            if (arr != null)
            {
                int ties = 0;
                foreach (var peer in arr)
                {
                    var cli = new NodeClient(peer);

                    connectors.Add(cli);

                    // init current block id
                    // await o.PeekLastBlockAsync(dc);

                    ties++;
                }

                if (ties >= 2)
                {
                    hub = true;
                }
            }


            // start the replicator thead
            replicator = new Thread(Replicate)
            {
                Name = "Replicator"
            };
            replicator.Start();
        }


        #region Db-Source-And-Cache

        public static DbSource DbSource => dbSource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbSource == null) // check on-the-fly
            {
                throw new DbException("missing 'home' in app.json");
            }

            return dbSource.NewDbContext(level);
        }

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
            if (objectCaches == null)
            {
                objectCaches = new List<DbCache>(16);
            }
            objectCaches.Add(new DbObjectCache<K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheMap<S, K, V>(Func<DbContext, S, Map<K, V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (mapCaches == null)
            {
                mapCaches = new List<DbCache>(8);
            }
            mapCaches.Add(new DbMapCache<S, K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheMap<S, K, V>(Func<DbContext, S, Task<Map<K, V>>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (mapCaches == null)
            {
                mapCaches = new List<DbCache>();
            }
            mapCaches.Add(new DbMapCache<S, K, V>(fetcher, typeof(V), maxage, flag));
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
            if (objectCaches == null)
            {
                return default;
            }
            foreach (var ca in objectCaches)
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
            if (objectCaches == null)
            {
                return default;
            }
            foreach (var ca in objectCaches)
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
            if (mapCaches == null)
            {
                return null;
            }
            foreach (var ca in mapCaches)
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
            if (mapCaches == null)
            {
                return null;
            }
            foreach (var ca in mapCaches)
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

        #endregion

        #region Nodal-Of-Network

        public static Peer Self => self;

        public static bool IsHub => hub;

        //
        // transaction id generator

        static int lastId = 0;

        public static int AutoInc()
        {
            return Interlocked.Increment(ref lastId);
        }

        public static NodeClient GetConnector(short peerid) => connectors[peerid];

        public static Map<short, NodeClient> Connectors => connectors;


        public static NodeContext NewNodeContext(short peerid = 0, IsolationLevel? level = null)
        {
            if (dbSource == null)
            {
                throw new NodeException("missing 'home' in app.json");
            }

            var cli = GetConnector(peerid);
            var cc = new NodeContext(dbSource, cli)
            {
                self = Self,
            };

            if (level != null)
            {
                cc.Begin(level.Value);
            }

            return cc;
        }

        #endregion

        //
        // replicate ledgers for remote peer
        //

        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;

        static async void Replicate(object state)
        {
            while (true)
            {
                Cycle:
                var outer = true;

                Thread.Sleep(60 * 1000);

                for (int i = 0; i < connectors.Count; i++) // LOOP
                {
                    var cli = connectors.ValueAt(i);

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
    }
}