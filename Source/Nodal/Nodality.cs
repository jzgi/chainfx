using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace ChainFx.Nodal
{
    /// <summary>
    /// The structure & environment for database and distributed ledger. 
    /// </summary>
    public abstract class Nodality
    {
        // db
        //

        static DbSource dbSource;

        static List<DbCache> caches; // an entire map (standard)

        static List<DbCache> objectCaches; // once an object a time

        static List<DbCache> mapCaches; // many a map


        // peer connectivty
        //

        static Node self;

        static readonly ConcurrentDictionary<short, Node> peers = new ConcurrentDictionary<short, Node>();

        // establised connectors
        static readonly ConcurrentDictionary<short, NodeClient> okayed = new ConcurrentDictionary<short, NodeClient>();


        public static void MapComposite<T>(string dbTyp = null)
        {
            if (dbTyp == null)
            {
                dbTyp = typeof(T).Name.ToLower();
            }
            NpgsqlConnection.GlobalTypeMapper.MapComposite<T>(dbTyp);
        }


        internal static void InitializeNodal(JObj fabriccfg)
        {
            // create db source
            dbSource = new DbSource(fabriccfg);

            // create self peer info
            self = new Node(fabriccfg)
            {
            };

            // load  peer connectors
            //
            // using var dc = NewDbContext();
            // dc.Sql("SELECT ").collst(Node.Empty).T(" FROM _peers_ WHERE status > 0");
            // var arr = dc.Query<Node>();
            // if (arr != null)
            // {
            //     foreach (var peer in arr)
            //     {
            //         var cli = new NodeClient(peer);
            //
            //         okayed.TryAdd(cli.Key, cli);
            //
            //         // init current block id
            //         // await o.PeekLastBlockAsync(dc);
            //     }
            // }

            // start the puller thead
            // puller = new Thread(Replicate)
            // {
            //     Name = "Block Puller"
            // };
            // puller.Start();
        }


        #region DB-API

        public static DbSource DbSource => dbSource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbSource == null)
            {
                throw new DbException("missing 'fabric' in app.json");
            }

            var dc = new DbContext();
            if (level != null)
            {
                dc.Begin(level.Value);
            }

            return dc;
        }

        public static void Cache<K, V>(Func<DbContext, Map<K, V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (caches == null)
            {
                caches = new List<DbCache>(16);
            }
            caches.Add(new DbCache<K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void Cache<K, V>(Func<DbContext, Task<Map<K, V>>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
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

        public static void CacheObject<K, V>(Func<DbContext, K, Task<V>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
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

        public static Map<K, V> Grab<K, V>(short flag = 0) where K : IComparable<K>
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

        public static async Task<Map<K, V>> GrabAsync<K, V>(short flag = 0) where K : IComparable<K>
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

        public static V GrabObject<K, V>(K key, short flag = 0) where K : IComparable<K>
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

        public static async Task<V> GrabObjectAsync<K, V>(K key, short flag = 0) where K : IComparable<K>
        {
            if (objectCaches == null)
            {
                return default;
            }
            foreach (var ca in objectCaches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return await ((DbObjectCache<K, V>) ca).GetAsync(key);
                    }
                }
            }
            return default;
        }


        public static Map<K, V> GrabMap<D, K, V>(D discr, short flag = 0) where K : IComparable<K>
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

        public static async Task<Map<K, V>> GrabMapAsync<D, K, V>(D discr, short flag = 0) where K : IComparable<K>
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


        #region NODAL-API

        public static Node Self => self;

        public static Node GetPeer(short key) => peers[key];

        //
        // transaction id generator

        static int lastId = 0;

        public static int AutoInc()
        {
            return Interlocked.Increment(ref lastId);
        }

        public static NodeClient GetConnector(short peerid) => okayed[peerid];

        public static ConcurrentDictionary<short, NodeClient> Okayed => okayed;


        public async Task AskAsync(Node node)
        {
            // validate peer

            // insert locally
            using (var dc = NewDbContext())
            {
                dc.Sql("INSERT INTO _peers_ ").colset(Node.Empty)._VALUES_(Node.Empty);
                await dc.ExecuteAsync(p => node.Write(p));
            }

            var connector = new NodeClient(node);

            // remote req
            node.id = Self.id;
            var (code, err) = await connector.AskAsync(node);
        }


        public static LdgrContext NewLdgrContext(short toPeerId = 0, IsolationLevel? level = IsolationLevel.ReadCommitted)
        {
            if (dbSource == null)
            {
                throw new LdgrException("missing 'fabric' in app.json");
            }

            var cli = toPeerId > 0 ? GetConnector(toPeerId) : null;
            var lc = new LdgrContext(cli)
            {
            };

            if (level != null)
            {
                lc.Begin(level.Value);
            }

            return lc;
        }

        #endregion

        //
        // replicate ledgers for remote peer
        //
        // periodically pulling of blocks of remote ledger  records
        static Thread routine;


        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;

        public static void StartNetwork(bool hub = false)
        {
            if (hub)
            {
                routine = new Thread(HubCycle);
            }
            else
            {
                routine = new Thread(PeerCycle);
            }
            
            routine.Start();
        }

        static async void PeerCycle(object state)
        {
            while (true)
            {
                Cycle:
                var outer = true;

                Thread.Sleep(60 * 1000);

                foreach (var pair in okayed) // LOOP
                {
                    var cli = pair.Value;
                    if (!cli.Node.IsRunning) continue;

                    int busy = 0;
                    var code = await cli.ReplicateAsync();
                    if (code == 200)
                    {
                        try
                        {
                            using var dc = NewDbContext(IsolationLevel.ReadCommitted);
                            long bchk = 0; // block checksum
                            for (short k = 0; k < cli.arr.Length; k++)
                            {
                                var o = cli.arr[k];

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
                                // if (i == 0) // begin of block 
                                // {
                                //     // if (o.blockcs != bchk) // compare with previous block
                                //     // {
                                //     //     cli.SetDataError("");
                                //     //     break;
                                //     // }
                                //
                                //     p.Set(bchk);
                                // }
                                // else if (i == arr.Count - 1) // end of block
                                // {
                                //     // validate block check
                                //     // if (bchk != o.blockcs)
                                //     // {
                                //     //     cli.SetDataError("");
                                //     // }
                                //     // p.Set(bchk = o.blockcs); // assign & set
                                // }
                                // else
                                // {
                                //     p.SetNull();
                                // }

                                await dc.SimpleExecuteAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            // cli.SetInternalError(e.Message);
                        }
                    }

                    if (code == 200 || (outer && (code == 0 || code == 204)))
                    {
                        // cli.ScheduleRemotePoll(0);
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

        static void HubCycle(object state)
        {
        }
    }
}