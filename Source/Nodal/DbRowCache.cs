using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChainFx.Nodal;

/// <summary>
/// A cache for multiple objects.
/// </summary>
internal class DbRowCache<K, V> : DbCache
{
    readonly bool async;

    readonly ConcurrentDictionary<K, (int, V)> cached = new();

    /// <summary>
    /// The sync version of constructor.
    /// </summary>
    /// <param name="fetch"></param>
    /// <param name="typ"></param>
    /// <param name="maxage"></param>
    /// <param name="flag"></param>
    internal DbRowCache(Func<DbContext, K, V> fetch, Type typ, int maxage, byte flag) : base(fetch, typ, maxage, flag)
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
    internal DbRowCache(Func<DbContext, K, Task<V>> fetch, Type typ, int maxage, byte flag) : base(fetch, typ, maxage, flag)
    {
        async = true;
    }

    public override bool IsAsync => async;


    public V Get(K key)
    {
        if (!(fetch is Func<DbContext, K, V> func)) // check fetcher
        {
            throw new DbException("Wrong fetcher for " + Typ);
        }
        var tick = Environment.TickCount & int.MaxValue; // positive tick

        var exist = cached.TryGetValue(key, out var ety);
        if (exist && tick < ety.Item1)
        {
            return ety.Item2;
        }

        // re-fetch
        using var dc = Nodality.NewDbContext();
        ety.Item1 = tick + MaxAge * 1000;
        ety.Item2 = func(dc, key);

        cached.AddOrUpdate(key, ety, (k, old) => ety); // re-cache

        return ety.Item2;
    }

    public async Task<V> GetAsync(K key)
    {
        if (!(fetch is Func<DbContext, K, Task<V>> func)) // check fetcher
        {
            throw new DbException("Wrong fetcher for " + Typ);
        }
        var tick = Environment.TickCount & int.MaxValue; // positive tick

        var exist = cached.TryGetValue(key, out var ety);
        if (exist && tick < ety.Item1)
        {
            return ety.Item2;
        }

        // re-fetch
        using var dc = Nodality.NewDbContext();
        ety.Item1 = tick + MaxAge * 1000;
        ety.Item2 = await func(dc, key);

        cached.AddOrUpdate(key, ety, (k, old) => ety); // re-cache

        return ety.Item2;
    }
}