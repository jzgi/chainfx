using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace ChainFx.Nodal
{
    /// <summary>
    /// The structure & environment for data store and digital twins. 
    /// </summary>
    public abstract class Nodality
    {
        // db store

        static DbSource dbSource;

        // cache

        static readonly List<DbCache> caches = new();

        // graph

        static readonly List<TwinGraph> graphs = new();


        public static void MapComposite<T>(string dbTyp = null)
        {
            if (dbTyp == null)
            {
                dbTyp = typeof(T).Name.ToLower();
            }
            NpgsqlConnection.GlobalTypeMapper.MapComposite<T>(dbTyp);
        }


        internal static void InitNodality(JObj dbcfg)
        {
            // create db source
            dbSource = new DbSource(dbcfg);
        }


        public static DbSource DbSource => dbSource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbSource == null)
            {
                throw new DbException("missing 'db' in app.json");
            }

            var dc = new DbContext();
            if (level != null)
            {
                dc.Begin(level.Value);
            }

            return dc;
        }


        #region Cache-API

        public static void MakeCache<K, V>(Func<DbContext, Map<K, V>> fetch, int maxage = 60, byte flag = 0)
            where K : IEquatable<K>, IComparable<K>

        {
            caches.Add(
                new DbSetCache<K, V>(fetch, typeof(V), maxage, flag)
            );
        }

        public static void MakeCache<K, V>(Func<DbContext, Task<Map<K, V>>> fetch, int maxage = 60, byte flag = 0)
            where K : IEquatable<K>, IComparable<K>

        {
            caches.Add(
                new DbSetCache<K, V>(fetch, typeof(V), maxage, flag)
            );
        }

        public static void MakeCache<K, V>(Func<DbContext, K, V> fetch, int maxage = 60, byte flag = 0)
            where K : IEquatable<K>, IComparable<K>

        {
            caches.Add(
                new DbKeyValueCache<K, V>(fetch, typeof(V), maxage, flag)
            );
        }

        public static void MakeCache<K, V>(Func<DbContext, K, Task<V>> fetch, int maxage = 60, byte flag = 0) 
            where K : IEquatable<K>, IComparable<K>

        {
            caches.Add(
                new DbKeyValueCache<K, V>(fetch, typeof(V), maxage, flag)
            );
        }

        public static void MakeCache<S, K, V>(Func<DbContext, S, Map<K, V>> fetch, int maxage = 60, byte flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            caches.Add(
                new DbKeySetCache<S, K, V>(fetch, typeof(V), maxage, flag)
            );
        }

        public static void MakeCache<S, K, V>(Func<DbContext, S, Task<Map<K, V>>> fetch, int maxage = 60, byte flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            caches.Add(
                new DbKeySetCache<S, K, V>(fetch, typeof(V), maxage, flag)
            );
        }

        public static Map<K, V> Grab<K, V>(short flag = 0) where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbSetCache<K, V>)ca).Get();
                    }
                }
            }
            return null;
        }

        public static async Task<Map<K, V>> GrabAsync<K, V>(short flag = 0) where K : IEquatable<K>, IComparable<K>

        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return await ((DbSetCache<K, V>)ca).GetAsync();
                    }
                }
            }
            return null;
        }

        public static V GrabValue<K, V>(K key, short flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbKeyValueCache<K, V>)ca).Get(key);
                    }
                }
            }
            return default;
        }

        public static async Task<V> GrabValueAsync<K, V>(K key, short flag = 0) where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return await ((DbKeyValueCache<K, V>)ca).GetAsync(key);
                    }
                }
            }
            return default;
        }

        public static bool InvalidateValue<K, V>(K key, short flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbKeyValueCache<K, V>)ca).Invalidate(key);
                    }
                }
            }
            return false;
        }


        public static Map<K, V> GrabSet<S, K, V>(S setkey, short flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbKeySetCache<S, K, V>)ca).Get(setkey);
                    }
                }
            }
            return null;
        }

        public static async Task<Map<K, V>> GrabSetAsync<S, K, V>(S setkey, short flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return await ((DbKeySetCache<S, K, V>)ca).GetAsync(setkey);
                    }
                }
            }
            return null;
        }

        public static bool InvalidateSet<S, K, V>(S setkey, short flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbKeySetCache<S, K, V>)ca).Invalidate(setkey);
                    }
                }
            }
            return false;
        }

        #endregion


        #region Graph-API

        protected static G MakeGraph<G>(string name, object state = null) where G : TwinGraph, new()
        {
            // create service (properties in order)
            var grh = new G
            {
                // Folder = folder ?? name
            };
            // svc.Init(prop, servicecfg);

            graphs.Add(grh);

            // invoke on creatte
            // grh.On();

            return grh;
        }

        public static G GetGraph<G, B, K, T>()
            where B : IEquatable<B>, IComparable<B>
            where K : IEquatable<K>, IComparable<K>
            where T : class, ITwin<B, K>
            where G : TwinGraph<B, K, T>
        {
            foreach (var graph in graphs)
            {
                if (typeof(T).IsAssignableFrom(graph.Typ))
                {
                    return (G)graph;
                }
            }
            return default;
        }

        public static T GrabTwin<G, K, T>(K key)
            where G : IEquatable<G>, IComparable<G>
            where K : IEquatable<K>, IComparable<K>
            where T : class, ITwin<G, K>
        {
            foreach (var graph in graphs)
            {
                if (typeof(T).IsAssignableFrom(graph.Typ))
                {
                    return ((TwinGraph<G, K, T>)graph).Get(key);
                }
            }
            return default;
        }

        public static T[] GrabTwinArray<G, K, T>(G gkey, Predicate<T> cond = null, Comparison<T> comp = null)
            where T : class, ITwin<G, K>
            where G : IEquatable<G>, IComparable<G>
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var graph in graphs)
            {
                if (typeof(T).IsAssignableFrom(graph.Typ))
                {
                    return ((TwinGraph<G, K, T>)graph).GetArray(gkey, cond, comp);
                }
            }
            return null;
        }

        public static Map<K, T> GrabTwinSet<G, K, T>(G gkey)
            where G : IEquatable<G>, IComparable<G>
            where K : IEquatable<K>, IComparable<K>
            where T : class, ITwin<G, K>
        {
            if (graphs == null)
            {
                return null;
            }

            foreach (var graph in graphs)
            {
                if (typeof(T).IsAssignableFrom(graph.Typ))
                {
                    return ((TwinGraph<G, K, T>)graph).GetMap(gkey);
                }
            }
            return null;
        }

        #endregion
    }
}