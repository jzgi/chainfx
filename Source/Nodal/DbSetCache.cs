using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChainFx.Nodal;

/// <summary>
/// A cache for multiple rowsets (maps) identified by their keys.
/// </summary>
internal class DbSetCache<M, K, V> : DbCache
{
    readonly bool async;

    readonly ConcurrentDictionary<M, (int, Map<K, V>)> cached = new ();


    internal DbSetCache(Func<DbContext, M, Map<K, V>> fetch, Type typ, int maxage, byte flag) : base(fetch, typ, maxage, flag)
    {
        async = false;
    }

    internal DbSetCache(Func<DbContext, M, Task<Map<K, V>>> fetch, Type typ, int maxage, byte flag) : base(fetch, typ, maxage, flag)
    {
        async = true;
    }

    public override bool IsAsync => async;


    public Map<K, V> Get(M key)
    {
        if (!(fetch is Func<DbContext, M, Map<K, V>> func)) // simple object
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

        cached.AddOrUpdate(key, ety, (k, old) => ety);

        return ety.Item2;
    }

    public async Task<Map<K, V>> GetAsync(M key)
    {
        if (!(fetch is Func<DbContext, M, Task<Map<K, V>>> func)) // check fetcher
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

        cached.AddOrUpdate(key, ety, (k, old) => ety);

        return ety.Item2;
    }
}