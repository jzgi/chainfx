using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SkyChain.Nodal
{
    /// <summary>
    /// The static environment for being a node of a federated blockchain network. 
    /// </summary>
    public abstract class Home
    {
        // db
        //

        static DbSource dbSource;

        static List<DbCache> caches;

        static List<DbCache> objectCaches;

        static List<DbCache> mapCaches;


        // connectivty
        //

        static Peer self;

        static readonly Map<short, NodeClient> nodeClients = new Map<short, NodeClient>(16);

        static bool hub;


        internal static void InitializeChainBase(JObj chaincfg)
        {
            // create db source
            dbSource = new DbSource(chaincfg);

            // create self peer info
            self = new Peer(chaincfg)
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

                    nodeClients.Add(cli);

                    // init current block id
                    // await o.PeekLastBlockAsync(dc);

                    ties++;
                }

                if (ties >= 2)
                {
                    hub = true;
                }
            }
        }


        #region Db-Source-And-Cache

        public static DbSource DbSource => dbSource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbSource == null) // check on-the-fly
            {
                throw new DbException("missing 'chain' in app.json");
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

        #region Federal-Locality

        public static Peer Self => self;

        public static bool IsHub => hub;

        //
        // transaction id generator

        static int lastId = 0;

        public static int AutoInc()
        {
            return Interlocked.Increment(ref lastId);
        }

        public static NodeClient GetConnector(short peerid) => nodeClients[peerid];

        public static Map<short, NodeClient> Connectors => nodeClients;


        public static async void NewFedRequest(Peer peer)
        {
            // create temporary fed connector
            var cli = new NodeClient(peer);

            var fc = NewNodeContext(peer.id);

            // insert
            fc.Sql("INSERT INTO peers_").colset(Peer.Empty)._VALUES_(Peer.Empty);
            await fc.ExecuteAsync(p => peer.Write(p));

            // remote req
            // fc.
        }


        public static NodeContext NewNodeContext(short peerid = 0, IsolationLevel? level = null)
        {
            if (dbSource == null)
            {
                throw new NodeException("missing 'chain' in app.json");
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
        // instance scope
        //

        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;
    }
}