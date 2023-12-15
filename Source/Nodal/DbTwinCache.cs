using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ChainFX.Nodal;

/// <summary>
/// An abstract implementation for twin cache. A concrete subclass should override callback methods to provide detail operations.
/// </summary>
/// <typeparam name="FK">the type of fork-key</typeparam>
/// <typeparam name="T">the type of the twin objects in the cache</typeparam>
public abstract class DbTwinCache<FK, T> : DbCache where FK : IEquatable<FK>, IComparable<FK> where T : class, ITwin<FK>
{
    // all twins
    private readonly ConcurrentDictionary<int, T> all = new();

    // grouped by key
    private readonly ConcurrentDictionary<FK, Map<int, T>> forks = new();

    // cycle handler
    private readonly Thread handler;

    protected DbTwinCache() : base(null, null, 0, 0)
    {
        handler = new Thread(HandlerStart)
        {
            Name = "Twin Cache" + typeof(T).Name
        };

        // start the thread
        handler.Start();
    }

    private async void HandlerStart()
    {
        // repeated execution
        while (true)
        {
            Thread.Sleep(Period * 1000);

            // group by group
            int sum = 0;
            foreach (var pair in forks)
            {
                sum += await ForkIoCycleAsync(pair.Key, pair.Value);
            }
        }
    }

    protected Thread Handler => handler;

    public override Type Typ => typeof(T);

    public int Period { get; set; } = 60;

    protected virtual Task<int> ForkIoCycleAsync(FK forkKey, Map<int, T> fork)
    {
        return Task.FromResult(0);
    }

    public async Task<T> CreateAsync(Func<DbContext, Task<T>> dbfunc)
    {
        using var dc = Nodality.NewDbContext();

        var twin = await dbfunc(dc);

        if (twin != null)
        {
            all.TryAdd(twin.Key, twin);

            // add to loaded group
            var fkey = twin.ForkKey;
            if (forks.TryGetValue(fkey, out var map))
            {
                lock (map)
                {
                    map.Add(twin);
                }
            }
            else // load group, the target group is included
            {
                map = LoadFork(dc, twin.ForkKey);
                if (map != null)
                {
                    // index each of the group members
                    for (int i = 0; i < map.Count; i++)
                    {
                        var ety = map.EntryAt(i);
                        all.TryAdd(ety.Key, ety.Value);
                    }

                    // enlist the group
                    forks.TryAdd(fkey, map);
                }
            }
        }

        return twin;
    }

    public async Task<bool> UpdateAsync(T twin, Func<DbContext, Task<bool>> dbupd, Action<T> upd = null)
    {
        using var dc = Nodality.NewDbContext();

        // the resulted object after operation
        var ret = await dbupd(dc);

        if (upd != null)
        {
            lock (twin)
            {
                upd(twin);
            }
        }
        return ret;
    }

    public async Task<bool> RemoveAsync(T twin, Func<DbContext, Task<bool>> dbfunc)
    {
        using var dc = Nodality.NewDbContext();

        var dbok = await dbfunc(dc);

        if (dbok)
        {
            all.TryRemove(twin.Key, out _);

            if (forks.TryGetValue(twin.ForkKey, out var map))
            {
                lock (map)
                {
                    map.Add(twin.Key, default); // set to null
                }
            }
        }

        return false;
    }


    public T Get(int key)
    {
        // check if indexed
        if (!all.TryGetValue(key, out var value))
        {
            using var dc = Nodality.NewDbContext();

            // try get group key first 
            if (!TryGetForkKey(dc, key, out var fkey))
            {
                return default;
            }

            // ensure the same group is loaded
            if (!forks.TryGetValue(fkey, out var map))
            {
                map = LoadFork(dc, fkey);

                if (map == null)
                {
                    // must be something wrong
                    return default;
                }

                // index each of the group members
                for (int i = 0; i < map.Count; i++)
                {
                    var ety = map.EntryAt(i);
                    all.TryAdd(ety.Key, ety.Value);

                    // set return value
                    if (key.Equals(ety.key))
                    {
                        value = ety.Value;
                    }
                }
                // enlist the group
                forks.TryAdd(fkey, map);
            }
        }
        return value;
    }

    public Map<int, T> RemoveFork(FK forkKey)
    {
        if (forks.TryRemove(forkKey, out var map))
        {
            // remove each from all index
            lock (map)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    var key = map.KeyAt(i);

                    all.TryRemove(key, out var _);
                }
            }

            return map;
        }
        return null;
    }


    public T[] GetArray(FK fkey, Predicate<T> filter = null, Comparison<T> sorter = null)
    {
        if (!forks.TryGetValue(fkey, out var map))
        {
            using var dc = Nodality.NewDbContext();

            map = LoadFork(dc, fkey);
            if (map == null)
            {
                return null;
            }

            // index each of the group members
            for (int i = 0; i < map.Count; i++)
            {
                var ety = map.EntryAt(i);
                all.TryAdd(ety.Key, ety.Value);
            }

            // enlist the group
            forks.TryAdd(fkey, map);
        }

        lock (map) // synchronize on the entire set
        {
            var arr = map.All(filter);

            if (sorter != null && arr != null)
            {
                Array.Sort(arr, sorter);
            }
            return arr;
        }
    }

    public abstract bool TryGetForkKey(DbContext dc, int key, out FK forkKey);

    public abstract Map<int, T> LoadFork(DbContext dc, FK forkKey);
}