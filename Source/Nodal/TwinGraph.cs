using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ChainFx.Nodal;

public abstract class TwinGraph
{
    // the actual type cached (and to seek for)
    public abstract Type Typ { get; }

    // bitwise matcher
    public short Flag { get; set; }

    protected internal virtual void OnCreate()
    {
    }
}

public abstract class TwinGraph<S, T> : TwinGraph
    where S : IEquatable<S>, IComparable<S>
    where T : class, ITwin<S>
{
    // all twins
    readonly ConcurrentDictionary<int, T> all = new();

    // by group key
    readonly ConcurrentDictionary<S, Map<int, T>> groups = new();

    // index geographic
    TwinCell[] cells;


    // cycle handler
    readonly Thread handler;


    public override Type Typ => typeof(T);

    public int Period { get; set; } = 60;

    protected TwinGraph()
    {
        handler = new Thread(async () =>
            {
                // repeated execution
                while (true)
                {
                    Thread.Sleep(Period * 1000);

                    // settle group by group
                    int sum = 0;
                    foreach (var pair in groups)
                    {
                        sum += await TwinSetIoCycleAsync(pair.Key, pair.Value);
                    }
                }
            }
        )
        {
            Name = "Graph " + Typ.Name
        };

        // start the thread
        handler.Start();
    }

    protected virtual Task<int> TwinSetIoCycleAsync(S setkey, Map<int, T> set)
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
            var gkey = twin.SetKey;
            if (groups.TryGetValue(gkey, out var map))
            {
                lock (map)
                {
                    map.Add(twin);
                }
            }
            else // load group, the target group is included
            {
                map = LoadTwinSet(dc, twin.SetKey);
                if (map != null)
                {
                    // index each of the group members
                    for (int i = 0; i < map.Count; i++)
                    {
                        var ety = map.EntryAt(i);
                        all.TryAdd(ety.Key, ety.Value);
                    }

                    // enlist the group
                    groups.TryAdd(gkey, map);
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

            if (groups.TryGetValue(twin.SetKey, out var map))
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
            if (!TryGetTwinSetKey(dc, key, out var gkey))
            {
                return default;
            }

            // ensure the same group is loaded
            if (!groups.TryGetValue(gkey, out var map))
            {
                map = LoadTwinSet(dc, gkey);

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
                groups.TryAdd(gkey, map);
            }
        }
        return value;
    }

    public Map<int, T> RemoveSet(S gkey)
    {
        if (groups.TryRemove(gkey, out var map))
        {
            // remove each from the all index
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


    public T[] GetArray(S gkey, Predicate<T> filter = null, Comparison<T> sorter = null)
    {
        if (!groups.TryGetValue(gkey, out var map))
        {
            using var dc = Nodality.NewDbContext();

            map = LoadTwinSet(dc, gkey);

            if (map == null) return null;

            // index each of the group members
            for (int i = 0; i < map.Count; i++)
            {
                var ety = map.EntryAt(i);
                all.TryAdd(ety.Key, ety.Value);
            }

            // enlist the group
            groups.TryAdd(gkey, map);
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

    public abstract bool TryGetTwinSetKey(DbContext dc, int key, out S setkey);

    public abstract Map<int, T> LoadTwinSet(DbContext dc, S setkey);
}