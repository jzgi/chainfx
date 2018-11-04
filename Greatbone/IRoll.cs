using System;

namespace Greatbone
{
    public interface IRoll<K, V> where K : IEquatable<K>
    {
        K Key { get; set; }

        int Count { get; }

        V this[int index] { get; }

        void Add(V v);
    }

    public static class RollUtility
    {
        public static void RollUp<V, K, R>(this V[] array, Func<V, K> keyer, ref R[] up) where K : IEquatable<K> where R : IRoll<K, V>, new()
        {
            if (array == null) return;

            if (up != null) // roll on the given top
            {
                int ir = 0; // current idx of top rolls

                for (int i = 0; i < array.Length; i++)
                {
                    var v = array[i];
                    while (ir < up.Length && keyer(v).Equals(up[ir]))
                    {
                        ir++;
                    }
                    if (ir == up.Length) return;
                    up[ir].Add(v);
                }
            }
            else // roll up and create top as needed
            {
                var list = new ValueList<R>();
                R cur = default; // current
                for (int i = 0; i < array.Length; i++)
                {
                    var v = array[i];
                    var key = keyer(v);
                    if (cur != null && key.Equals(cur.Key))
                    {
                        cur.Add(v);
                    }
                    else // create a new roll
                    {
                        cur = new R {Key = key};
                        cur.Add(v);
                        list.Add(cur);
                    }
                }
                up = list.ToArray();
            }
        }
    }
}