using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class DbEnv
    {
        protected static DbSource dbsource;

        //
        // db-object cache
        //

        public static DbSource DbSource => dbsource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbsource == null)
            {
                throw new ServerException("missing 'db' in app.json");
            }

            return dbsource.NewDbContext(level);
        }

        //
        // Db Cache API
        //

        static List<DbCache> maps;

        static List<DbCache> valuesets;

        static List<DbCache> mapsets;

        internal static void ConfigureDb(JObj dbcfg)
        {
            dbsource = new DbSource(dbcfg);
        }

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
    }
}