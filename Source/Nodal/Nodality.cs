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
        // db data source
        static DbSource dbSource;

        // db object caches
        static readonly List<DbCache> caches = new();

        // twin graphs
        static readonly List<TwinGraph> graphs = new();


        /// <summary>
        /// To register a CLR type to map to a postgresql composite type.
        /// </summary>
        /// <param name="dbTyp">the composite type name</param>
        /// <typeparam name="T">the CLR type</typeparam>
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

        public static bool ExpireValue<K, V>(K key, short flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbKeyValueCache<K, V>)ca).Remove(key);
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

        public static bool ExpireSet<S, K, V>(S setkey, short flag = 0)
            where K : IEquatable<K>, IComparable<K>
        {
            foreach (var ca in caches)
            {
                if (ca.Flag == 0 || (ca.Flag & flag) > 0)
                {
                    if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                    {
                        return ((DbKeySetCache<S, K, V>)ca).Remove(setkey);
                    }
                }
            }
            return false;
        }

        #endregion


        #region Twin Graph-API

        protected static G MakeGraph<G>(string name, object state = null)
            where G : TwinGraph, new()
        {
            // create service (properties in order)
            var grh = new G
            {
                // Folder = folder ?? name
            };
            // svc.Init(prop, servicecfg);
            graphs.Add(grh);

            // oncreate
            grh.OnCreate();

            return grh;
        }

        public static G GetGraph<G, S, T>()
            where G : TwinGraph<S, T>
            where S : IEquatable<S>, IComparable<S>
            where T : class, ITwin<S>
        {
            foreach (var grh in graphs)
            {
                if (typeof(T).IsAssignableFrom(grh.Typ))
                {
                    return (G)grh;
                }
            }
            return default;
        }

        public static T GrabTwin<S, T>(int key)
            where S : IEquatable<S>, IComparable<S>
            where T : class, ITwin<S>
        {
            foreach (var grh in graphs)
            {
                if (typeof(T).IsAssignableFrom(grh.Typ))
                {
                    return ((TwinGraph<S, T>)grh).Get(key);
                }
            }
            return default;
        }

        public static T[] GrabTwinSet<S, T>(S gkey, Predicate<T> cond = null, Comparison<T> comp = null)
            where S : IEquatable<S>, IComparable<S>
            where T : class, ITwin<S>
        {
            foreach (var grh in graphs)
            {
                if (typeof(T).IsAssignableFrom(grh.Typ))
                {
                    return ((TwinGraph<S, T>)grh).GetArray(gkey, cond, comp);
                }
            }
            return null;
        }
    }

    #endregion
}