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

public abstract class TwinGraph<G, K, T> : TwinGraph
    where T : class, ITwin<G, K>
    where G : IEquatable<G>, IComparable<G>
    where K : IEquatable<K>, IComparable<K>
{
    // index for all
    readonly ConcurrentDictionary<K, T> all = new();

    // by group key
    readonly ConcurrentDictionary<G, Map<K, T>> groups = new();

    // index geographic
    TwinCell[] cells;


    readonly Thread worker;

    public override Type Typ => typeof(T);

    protected TwinGraph()
    {
        worker = new Thread(async (state) =>
        {
            Thread.Sleep(1000 * 30);

            // settle group by group
            int sum = 0;
            foreach (var pair in groups)
            {
                sum += await DischargeGroupAsync(pair.Key, pair.Value);
            }
        })
        {
            Name = "Graph " + Typ.Name
        };

        // start the thread
        worker.Start();
    }

    protected virtual async Task<int> DischargeGroupAsync(G gkey, Map<K, T> group)
    {
        return 0;
    }

    public async Task<T> CreateAsync(Func<DbContext, Task<T>> dbfunc)
    {
        using var dc = Nodality.NewDbContext();

        var twin = await dbfunc(dc);

        if (twin != null)
        {
            all.TryAdd(twin.Key, twin);

            // add to loaded group
            var gkey = twin.GroupKey;
            if (groups.TryGetValue(gkey, out var map))
            {
                lock (map)
                {
                    map.Add(twin);
                }
            }
            else // load group, the target group is included
            {
                map = LoadGroup(dc, twin.GroupKey);
                if (map != null)
                {
                    // index each of the group members
                    for (int i = 0; i < map.Count; i++)
                    {
                        var ety = map.EntryAt(i);
                        all.TryAdd(ety.Key, ety.value);
                    }

                    // enlist the group
                    groups.TryAdd(gkey, map);
                }
            }
        }

        return twin;
    }

    public async Task<bool> UpdateAsync(T twin, Func<DbContext, Task<bool>> dbfunc)
    {
        using var dc = Nodality.NewDbContext();

        // the resulted object after operation
        return await dbfunc(dc);
    }

    public async Task<bool> RemoveAsync(T twin, Func<DbContext, Task<bool>> dbfunc)
    {
        using var dc = Nodality.NewDbContext();

        var dbok = await dbfunc(dc);

        if (dbok)
        {
            all.TryRemove(twin.Key, out _);

            if (groups.TryGetValue(twin.GroupKey, out var map))
            {
                lock (map)
                {
                    map.Add(twin.Key, default); // set to null
                }
            }
        }

        return false;
    }


    public T Get(K key)
    {
        // check if indexed
        if (!all.TryGetValue(key, out var value))
        {
            using var dc = Nodality.NewDbContext();

            // try get group key first 
            if (!TryGetGroupKey(dc, key, out var gkey))
            {
                return default;
            }

            // ensure the same group is loaded
            if (!groups.TryGetValue(gkey, out var map))
            {
                map = LoadGroup(dc, gkey);

                if (map != null)
                {
                    // index each of the group members
                    for (int i = 0; i < map.Count; i++)
                    {
                        var ety = map.EntryAt(i);
                        all.TryAdd(ety.Key, ety.value);
                    }

                    // enlist the group
                    groups.TryAdd(gkey, map);
                }
            }
        }
        return value;
    }

    public Map<K, T> GetGroup(G gkey)
    {
        if (groups.TryGetValue(gkey, out var v))
        {
            return v;
        }
        return null;
    }

    public Map<K, T> RemoveGroup(G gkey)
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


    public T[] GetArray(G gkey, Predicate<T> cond = null, Comparison<T> comp = null)
    {
        if (!groups.TryGetValue(gkey, out var map))
        {
            using var dc = Nodality.NewDbContext();

            map = LoadGroup(dc, gkey);

            if (map == null) return null;

            // index each of the group members
            for (int i = 0; i < map.Count; i++)
            {
                var ety = map.EntryAt(i);
                all.TryAdd(ety.Key, ety.value);
            }

            // enlist the group
            groups.TryAdd(gkey, map);
        }

        T[] arr;
        lock (map)
        {
            arr = map.All(cond);
        }

        if (comp != null && arr != null)
        {
            Array.Sort(arr, comp);
        }
        return arr;
    }

    public abstract bool TryGetGroupKey(DbContext dc, K key, out G gkey);

    public abstract Map<K, T> LoadGroup(DbContext dc, G gkey);
}