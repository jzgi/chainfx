using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using SkyChain.Web;

namespace SkyChain.Store
{
    public class Home
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
                throw new DbException("missing 'store' in json");
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


        // local info
        static Peer info;

        // chainable table structures   
        static readonly Map<string, FedTable> tables = new Map<string, FedTable>(16);

        // remote connectors 
        static readonly Map<short, FedClient> clients = new Map<short, FedClient>(16);


        internal static void InitializeStore(JObj storecfg)
        {
            dbsource = new DbSource(storecfg);

            // load local info
            info = new Peer(storecfg);

            // setup chainables
            //
            using (var dc = NewDbContext())
            {
                // tables
                dc.Query("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'");
                while (dc.Next())
                {
                    dc.Let(out string table_name);
                    if (table_name.EndsWith('_') && table_name != "peers_")
                    {
                        tables.Add(new FedTable(table_name));
                    }
                }
                // columns for each
                for (int i = 0; i < tables.Count; i++)
                {
                    var tbl = tables.ValueAt(i);
                    dc.Sql("SELECT data_type, column_name, column_default, is_nullable, character_maximum_length FROM information_schema.columns WHERE table_schema = 'public' AND table_name = @1");
                    dc.Query(p => p.Set(tbl.Key));
                    // while (dc.Next())
                    // {
                    //     dc.Let(out string datatype);
                    //     FedColumn conn = datatype switch
                    //     {
                    //         "smallint" => new SmallintColumn(),
                    //         "int" => new IntColumn(),
                    //     };
                    //     conn.Read(dc);
                    //     // tables.Add(conn);
                    // }

                    // init current block id
                    // await o.PeekLastBlockAsync(dc);
                }

                // type mapping
                NpgsqlConnection.GlobalTypeMapper.MapComposite<Act>("act_type");
            }

            // setup remotes
            //
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM peers_");
                var arr = dc.Query<Peer>();
                if (arr != null)
                {
                    foreach (var peer in arr)
                    {
                        var cli = new FedClient(peer);
                        clients.Add(cli);

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


        public static FedClient GetClient(short peerid) => clients[peerid];

        public static Map<short, FedClient> Clients => clients;

        public static FedContext NewChainContext(WebContext wc, short peerid = 0, IsolationLevel? level = null)
        {
            if (DbSource == null)
            {
                throw new ApplicationException("missing 'chain' in app.json");
            }

            return DbSource.NewChainContext(wc, peerid, level);
        }


        //
        // instance scope
        //

        const int MAX_BLOCK_SIZE = 64;

        const int MIN_BLOCK_SIZE = 8;
    }
}