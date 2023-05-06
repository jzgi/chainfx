using System;
using System.Collections.Concurrent;

namespace ChainFx.Nodal;

public abstract class TwinGraph<T> : TwinGraph where T : ITwin
{
    // index for all
    readonly ConcurrentDictionary<int, T> all = new();

    // keyed by setkey
    readonly ConcurrentDictionary<int, Map<int, T>> sets = new();

    // index geographic
    TwinCell[] cells;


    public override Type Typ => typeof(T);


    public T Get(int key)
    {
        if (!all.TryGetValue(key, out var v))
        {
            using var dc = Nodality.NewDbContext();

            // load 
            v = Load(dc, key);

            if (v == null) return v;

            var setkey = v.TwinSetKey;

            // load the same group

            var map = LoadMap(dc, setkey);

            if (map != null)
            {
                sets.TryAdd(setkey, map);

                for (int i = 0; i < map.Count; i++)
                {
                    var ety = map.EntryAt(i);
                    all.TryAdd(ety.Key, ety.value);
                }
            }
        }
        return v;
    }

    public Map<int, T> GetMap(int setkey)
    {
        if (sets.TryGetValue(setkey, out var v))
        {
            return v;
        }
        return null;
    }


    public T[] GetArray(int setkey, Predicate<T> filter = null, Comparison<T> comp = null)
    {
        if (!sets.TryGetValue(setkey, out var map))
        {
            using var dc = Nodality.NewDbContext();

            map = LoadMap(dc, setkey);

            if (map == null)
            {
                return null;
            }

            sets.TryAdd(setkey, map);
        }

        var arr = map.All(filter);

        if (comp != null)
        {
            Array.Sort(arr, comp);
        }
        return arr;
    }

    public abstract T Load(DbContext dc, int key);

    public abstract Map<int, T> LoadMap(DbContext dc, int setkey);

    public void Save()
    {
    }
}