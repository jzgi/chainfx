using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace ChainFX.Nodal;

/// <summary>
/// The structure & environment for data store and digital twins. 
/// </summary>
public abstract class Nodality
{
    // db data source
    private static DbSource dbSource;

    // db object caches
    private static readonly List<DbCache> maps = new();

    private static readonly List<DbCache> valueCaches = new();

    private static readonly List<DbCache> grouperCaches = new();

    // twin caches
    private static readonly List<DbCache> twinCaches = new();


    /// <summary>
    /// To register a CLR type to map to a postgresql composite type.
    /// </summary>
    /// <param name="dbTyp">the composite type name</param>
    /// <typeparam name="T">the CLR type</typeparam>
    public static void MapCompositeDbType<T>(string dbTyp = null)
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


    public static void MakeCache<K, V>(Func<DbContext, Map<K, V>> fetch, int maxage = 60, byte flag = 0)
        where K : IEquatable<K>, IComparable<K>

    {
        maps.Add(
            new DbMap<K, V>(fetch, typeof(V), maxage, flag)
        );
    }

    public static void MakeCache<K, V>(Func<DbContext, Task<Map<K, V>>> fetch, int maxage = 60, byte flag = 0)
        where K : IEquatable<K>, IComparable<K>

    {
        maps.Add(
            new DbMap<K, V>(fetch, typeof(V), maxage, flag)
        );
    }

    public static void MakeCache<K, V>(Func<DbContext, K, V> fetch, int maxage = 60, byte flag = 0)
        where K : IEquatable<K>, IComparable<K>

    {
        valueCaches.Add(
            new DbValueCache<K, V>(fetch, typeof(V), maxage, flag)
        );
    }

    public static void MakeCache<K, V>(Func<DbContext, K, Task<V>> fetch, int maxage = 60, byte flag = 0)
        where K : IEquatable<K>, IComparable<K>

    {
        valueCaches.Add(
            new DbValueCache<K, V>(fetch, typeof(V), maxage, flag)
        );
    }

    public static void MakeCache<GK, K, V>(Func<DbContext, GK, Map<K, V>> fetch, int maxage = 60, byte flag = 0)
        where K : IEquatable<K>, IComparable<K>
    {
        grouperCaches.Add(
            new DbGrouperCache<GK, K, V>(fetch, typeof(V), maxage, flag)
        );
    }

    public static void MakeCache<GK, K, V>(Func<DbContext, GK, Task<Map<K, V>>> fetch, int maxage = 60, byte flag = 0)
        where K : IEquatable<K>, IComparable<K>
    {
        grouperCaches.Add(
            new DbGrouperCache<GK, K, V>(fetch, typeof(V), maxage, flag)
        );
    }

    public static Map<K, V> Grab<K, V>(short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in maps)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return ((DbMap<K, V>)ca).Get();
                }
            }
        }
        return null;
    }

    public static async Task<Map<K, V>> GrabAsync<K, V>(short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in maps)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return await ((DbMap<K, V>)ca).GetAsync();
                }
            }
        }
        return null;
    }

    public static bool Expire<K, V>(K key, short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in maps)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    // return ((DbMap<K, V>)ca).Get();
                }
            }
        }
        return false;
    }

    public static V GrabValue<K, V>(K key, short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in valueCaches)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return ((DbValueCache<K, V>)ca).Get(key);
                }
            }
        }
        return default;
    }

    public static async Task<V> GrabValueAsync<K, V>(K key, short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in valueCaches)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return await ((DbValueCache<K, V>)ca).GetAsync(key);
                }
            }
        }
        return default;
    }

    public static bool ExpireValue<K, V>(K key, short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in valueCaches)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return ((DbValueCache<K, V>)ca).Remove(key);
                }
            }
        }
        return false;
    }

    public static Map<K, V> GrabGroup<GK, K, V>(GK groupKey, short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in grouperCaches)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return ((DbGrouperCache<GK, K, V>)ca).Get(groupKey);
                }
            }
        }
        return null;
    }

    public static async Task<Map<K, V>> GrabGroupAsync<GK, K, V>(GK groupKey, short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in grouperCaches)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return await ((DbGrouperCache<GK, K, V>)ca).GetAsync(groupKey);
                }
            }
        }
        return null;
    }

    public static bool ExpireGroup<GK, K, V>(GK groupKey, short flag = 0) where K : IEquatable<K>, IComparable<K>
    {
        foreach (var ca in grouperCaches)
        {
            if (ca.Flag == 0 || (ca.Flag & flag) > 0)
            {
                if (!ca.IsAsync && typeof(V).IsAssignableFrom(ca.Typ))
                {
                    return ((DbGrouperCache<GK, K, V>)ca).Remove(groupKey);
                }
            }
        }
        return false;
    }


    #region Twin Cache API

    public static T MakeCache<T>(string name, object state = null) where T : DbCache, new()
    {
        // create service (properties in order)
        var grh = new T
        {
            // Folder = folder ?? name
        };
        // svc.Init(prop, servicecfg);
        twinCaches.Add(grh);

        // oncreate
        // grh.OnCreate();

        return grh;
    }

    /// <summary>
    /// Gets 
    /// </summary>
    /// <typeparam name="C">The cache class</typeparam>
    /// <typeparam name="FK">fork key type</typeparam>
    /// <typeparam name="T">twin class</typeparam>
    /// <returns></returns>
    public static C GetTwinCache<C, FK, T>() where C : DbTwinCache<FK, T> where FK : IEquatable<FK>, IComparable<FK> where T : class, ITwin<FK>
    {
        foreach (var ca in twinCaches)
        {
            if (typeof(T).IsAssignableFrom(ca.Typ))
            {
                return (C)ca;
            }
        }
        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="FK"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GrabTwin<FK, T>(int key) where FK : IEquatable<FK>, IComparable<FK> where T : class, ITwin<FK>
    {
        foreach (var ca in twinCaches)
        {
            if (typeof(T).IsAssignableFrom(ca.Typ))
            {
                return ((DbTwinCache<FK, T>)ca).Get(key);
            }
        }
        return default;
    }

    public static T[] GrabTwinArray<FK, T>(FK forkKey, Predicate<T> filter = null, Comparison<T> sorter = null)
        where FK : IEquatable<FK>, IComparable<FK>
        where T : class, ITwin<FK>
    {
        foreach (var ca in twinCaches)
        {
            if (typeof(T).IsAssignableFrom(ca.Typ))
            {
                return ((DbTwinCache<FK, T>)ca).GetArray(forkKey, filter, sorter);
            }
        }
        return null;
    }

    #endregion
}