using System;
using System.Collections.Concurrent;

namespace ChainFx.Nodal;

public abstract class TwinGraph
{
    // the actual type cached (and to seek for)
    public abstract Type Typ { get; }

    // bitwise matcher
    public short Flag { get; set; }
}

public abstract class TwinGraph<G, K, T> : TwinGraph where T : ITwin<G, K>
    where G : IEquatable<G>, IComparable<G>
    where K : IEquatable<K>, IComparable<K>
{
    // index for all
    readonly ConcurrentDictionary<K, T> all = new();

    // by group key
    readonly ConcurrentDictionary<G, Map<K, T>> groups = new();

    // index geographic
    TwinCell[] cells;


    public override Type Typ => typeof(T);


    public T Get(K key)
    {
        if (!all.TryGetValue(key, out var v))
        {
            using var dc = Nodality.NewDbContext();

            // load 
            v = Load(dc, key);

            if (v == null) return v;

            var gkey = v.GroupKey;

            // load the same group

            var map = LoadGroup(dc, gkey);

            if (map != null)
            {
                groups.TryAdd(gkey, map);

                for (int i = 0; i < map.Count; i++)
                {
                    var ety = map.EntryAt(i);
                    all.TryAdd(ety.Key, ety.value);
                }
            }
        }
        return v;
    }

    public void Add(T v)
    {
        var key = v.Key;

        if (all.TryGetValue(key, out _))
        {
            return;
        }

        using var dc = Nodality.NewDbContext();
        var gkey = v.GroupKey;
        if (!groups.TryGetValue(gkey, out var group))
        {
            group = LoadGroup(dc, gkey);

            if (group != null)
            {
                groups.TryAdd(gkey, group);

                for (int i = 0; i < group.Count; i++)
                {
                    var ety = group.EntryAt(i);
                    all.TryAdd(ety.Key, ety.value);
                }
            }
            return;
        }

        group.Add(key, v);
    }

    public Map<K, T> GetMap(G gkey)
    {
        if (groups.TryGetValue(gkey, out var v))
        {
            return v;
        }
        return null;
    }

    public Map<K, T> DropMap(G gkey)
    {
        if (groups.TryRemove(gkey, out var v))
        {
            return v;
        }
        return null;
    }


    public T[] GetArray(G gkey, Predicate<T> cond = null, Comparison<T> comp = null)
    {
        if (!groups.TryGetValue(gkey, out var map))
        {
            using var dc = Nodality.NewDbContext();

            map = LoadGroup(dc, gkey);

            if (map == null)
            {
                return null;
            }

            groups.TryAdd(gkey, map);
        }

        var arr = map.All(cond);

        if (comp != null && arr != null)
        {
            Array.Sort(arr, comp);
        }
        return arr;
    }

    public abstract T Load(DbContext dc, K key);

    public abstract Map<K, T> LoadGroup(DbContext dc, G gkey);

    public abstract bool Save(DbContext dc, T v);

    public abstract bool Remove(DbContext dc, K key);
}