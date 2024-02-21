using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChainFX.Nodal;

/// <summary>
/// A cache for key-value pairs retrived from database..
/// </summary>
/// <typeparam name="K">the type of the keys in the cache</typeparam>
/// <typeparam name="V">the type of the values in the cache</typeparam>
internal class DbValueCache<K, V> : DbCache where K : IComparable<K>, IEquatable<K>
{
    private readonly bool async;

    private readonly ConcurrentDictionary<K, (int, V)> data = new();

    /// <summary>
    /// The sync version of constructor.
    /// </summary>
    /// <param name="fetch"></param>
    /// <param name="typ"></param>
    /// <param name="maxage"></param>
    /// <param name="flag"></param>
    internal DbValueCache(Func<DbContext, K, V> fetch, Type typ, int maxage, byte flag) :
        base(fetch, typ, maxage, flag)
    {
        async = false;
    }

    /// <summary>
    /// The async version of constructor.
    /// </summary>
    /// <param name="fetch"></param>
    /// <param name="typ"></param>
    /// <param name="maxage"></param>
    /// <param name="flag"></param>
    internal DbValueCache(Func<DbContext, K, Task<V>> fetch, Type typ, int maxage, byte flag) :
        base(fetch, typ, maxage, flag)
    {
        async = true;
    }

    public override bool IsAsync => async;


    public V Get(K key)
    {
        if (!(fetch is Func<DbContext, K, V> func)) // check fetcher
        {
            throw new DbCacheException("incorrect fetcher for " + Typ);
        }
        var tick = Environment.TickCount & int.MaxValue; // positive tick

        var hit = data.TryGetValue(key, out var ety);
        if (hit && tick < ety.Item1)
        {
            return ety.Item2;
        }

        // re-fetch
        using var dc = Storage.NewDbContext();
        ety.Item1 = tick + MaxAge * 1000;
        ety.Item2 = func(dc, key);

        data.AddOrUpdate(key, ety, (_, _) => ety); // re-cache

        return ety.Item2;
    }

    public async Task<V> GetAsync(K key)
    {
        if (!(fetch is Func<DbContext, K, Task<V>> func)) // check fetcher
        {
            throw new DbCacheException("incorrect fetcher for " + Typ);
        }

        var tick = Environment.TickCount & int.MaxValue; // positive tick

        var hit = data.TryGetValue(key, out var ety);
        if (hit && tick < ety.Item1)
        {
            return ety.Item2;
        }

        // re-fetch
        using var dc = Storage.NewDbContext();
        ety.Item1 = tick + MaxAge * 1000;
        ety.Item2 = await func(dc, key);

        data.AddOrUpdate(key, ety, (_, _) => ety); // re-cache

        return ety.Item2;
    }

    public bool Remove(K key)
    {
        return data.TryRemove(key, out _);
    }
}