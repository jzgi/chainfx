using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChainFx.Nodal;

public abstract class TwinGraph
{
    // the actual type cached (and to seek for)
    public abstract Type Typ { get; }

    // bitwise matcher
    public short Flag { get; set; }
}

public abstract class TwinGraph<B, K, T> : TwinGraph
    where T : class, ITwin<B, K>
    where B : IEquatable<B>, IComparable<B>
    where K : IEquatable<K>, IComparable<K>
{
    // index for all
    readonly ConcurrentDictionary<K, T> all = new();

    // by group key
    readonly ConcurrentDictionary<B, Map<K, T>> groups = new();

    // index geographic
    TwinCell[] cells;


    public override Type Typ => typeof(T);


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

            // load 
            value = Load(dc, key);

            if (value == null) return default;

            // ensure the same group is loaded
            var gkey = value.GroupKey;
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

    public Map<K, T> GetMap(B gkey)
    {
        if (groups.TryGetValue(gkey, out var v))
        {
            return v;
        }
        return null;
    }

    public Map<K, T> DropMap(B gkey)
    {
        if (groups.TryRemove(gkey, out var v))
        {
            return v;
        }
        return null;
    }


    public T[] GetArray(B gkey, Predicate<T> cond = null, Comparison<T> comp = null)
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

    public abstract T Load(DbContext dc, K key);

    public abstract Map<K, T> LoadGroup(DbContext dc, B gkey);
}