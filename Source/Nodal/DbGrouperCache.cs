using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChainFX.Nodal;

/// <summary>
/// A cache that divides data records into groups by group key.
/// </summary>
/// <typeparam name="GK">the type of group keys</typeparam>
/// <typeparam name="K">the type of the keys in the cache</typeparam>
/// <typeparam name="V">the type of the values in the cache</typeparam>
internal class DbGrouperCache<GK, K, V> : DbCache where K : IEquatable<K>, IComparable<K>
{
    private readonly bool async;

    private readonly ConcurrentDictionary<GK, (int, Map<K, V>)> data = new();


    internal DbGrouperCache(Func<DbContext, GK, Map<K, V>> fetch, Type typ, int maxage, byte flag) :
        base(fetch, typ, maxage, flag)
    {
        async = false;
    }

    internal DbGrouperCache(Func<DbContext, GK, Task<Map<K, V>>> fetch, Type typ, int maxage, byte flag) :
        base(fetch, typ, maxage, flag)
    {
        async = true;
    }

    public override bool IsAsync => async;


    public Map<K, V> Get(GK gkey)
    {
        if (!(fetch is Func<DbContext, GK, Map<K, V>> func)) // simple object
        {
            throw new DbCacheException("incorrect fetcher for " + Typ);
        }

        var tick = Environment.TickCount & int.MaxValue; // positive tick

        var hit = data.TryGetValue(gkey, out var ety);
        if (hit && tick < ety.Item1)
        {
            return ety.Item2;
        }

        // re-fetch
        using var dc = Storage.NewDbContext();
        ety.Item1 = tick + MaxAge * 1000;
        ety.Item2 = func(dc, gkey);

        data.AddOrUpdate(gkey, ety, (_, _) => ety);

        return ety.Item2;
    }

    public async Task<Map<K, V>> GetAsync(GK gkey)
    {
        if (!(fetch is Func<DbContext, GK, Task<Map<K, V>>> func)) // check fetcher
        {
            throw new DbCacheException("incorrect fetcher for " + Typ);
        }

        var tick = Environment.TickCount & int.MaxValue; // positive tick

        var hit = data.TryGetValue(gkey, out var ety);
        if (hit && tick < ety.Item1)
        {
            return ety.Item2;
        }

        // re-fetch
        using var dc = Storage.NewDbContext();
        ety.Item1 = tick + MaxAge * 1000;
        ety.Item2 = await func(dc, gkey);

        data.AddOrUpdate(gkey, ety, (_, _) => ety);

        return ety.Item2;
    }


    public bool Remove(GK key)
    {
        return data.TryRemove(key, out _);
    }
}