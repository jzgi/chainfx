using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class DbEnviron
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

        static List<DbCache> valuecolls;

        static List<DbCache> mapcolls;

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
            maps.Add(new DbMapEx<K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheMap<K, V, D>(Func<DbContext, Map<K, V>> fetcher, Func<V, D> discr = null, int maxage = 60, byte flag = 0) where K : IComparable<K> where D : IComparable<D>
        {
            if (maps == null)
            {
                maps = new List<DbCache>(16);
            }
            maps.Add(new DbMap<K, V, D>(fetcher, discr, typeof(V), maxage, flag));
        }

        public static void Cache<K, V>(Func<DbContext, K, V> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (valuecolls == null)
            {
                valuecolls = new List<DbCache>(16);
            }
            valuecolls.Add(new DbCollection<K, V>(fetcher, typeof(V), maxage, flag));
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
            if (mapcolls == null)
            {
                mapcolls = new List<DbCache>();
            }
            mapcolls.Add(new DbMapCollection<S, K, V>(fetcher, typeof(V), maxage, flag));
        }

        public static void CacheSub<S, K, V>(Func<DbContext, S, Task<Map<K, V>>> fetcher, int maxage = 60, byte flag = 0) where K : IComparable<K>
        {
            if (mapcolls == null)
            {
                mapcolls = new List<DbCache>();
            }
            mapcolls.Add(new DbMapCollection<S, K, V>(fetcher, typeof(V), maxage, flag));
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
                        return ((DbMapEx<K, V>) cache).Get();
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
                        return await ((DbMapEx<K, V>) cache).GetAsync();
                    }
                }
            }
            return null;
        }

        public static Lot<D, V> ObtainLot<K, V, D>(byte flag = 0) where K : IComparable<K> where D : IComparable<D>
        {
            if (maps != null)
            {
                foreach (var cache in maps)
                {
                    if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                    {
                        if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                        {
                            return ((DbMap<K, V, D>) cache).GetSort();
                        }
                    }
                }
            }
            return null;
        }

        public static async Task<Lot<D, V>> ObtainLotAsync<K, V, D>(byte flag = 0) where K : IComparable<K> where D : IComparable<D>
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
                        return await ((DbMap<K, V, D>) cache).GetLotAsync();
                    }
                }
            }
            return null;
        }

        public static V Obtain<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            if (valuecolls == null)
            {
                return default;
            }
            foreach (var cache in valuecolls)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return ((DbCollection<K, V>) cache).Get(key);
                    }
                }
            }
            return default;
        }

        public static async Task<V> ObtainAsync<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            if (valuecolls == null)
            {
                return default;
            }
            foreach (var cache in valuecolls)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return await ((DbCollection<K, V>) cache).GetAsync(key);
                    }
                }
            }
            return default;
        }


        public static Map<K, V> ObtainSub<D, K, V>(D discr, byte flag = 0) where K : IComparable<K>
        {
            if (mapcolls == null)
            {
                return null;
            }
            foreach (var cache in mapcolls)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (!cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return ((DbMapCollection<D, K, V>) cache).Get(discr);
                    }
                }
            }
            return null;
        }

        public static async Task<Map<K, V>> ObtainSubAsync<D, K, V>(D discr, byte flag = 0) where K : IComparable<K>
        {
            if (mapcolls == null)
            {
                return null;
            }
            foreach (var cache in mapcolls)
            {
                if (cache.Flag == 0 || (cache.Flag & flag) > 0)
                {
                    if (cache.IsAsync && typeof(V).IsAssignableFrom(cache.Typ))
                    {
                        return await ((DbMapCollection<D, K, V>) cache).GetAsync(discr);
                    }
                }
            }
            return null;
        }
    }
}