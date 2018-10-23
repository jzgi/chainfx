using System;

namespace Greatbone
{
    public class Roll<K, V> : IKeyable<K>
    {
        const int DefaultCap = 16;

        V[] array;

        int count;

        public Roll()
        {
        }

        public K Key { get; internal set; }

        protected internal virtual void Add(V v)
        {
            // ensure capacity
            if (array == null)
            {
                array = new V[DefaultCap];
            }
            else
            {
                int len = array.Length;
                if (count >= len)
                {
                    V[] alloc = new V[len * 4];
                    Array.Copy(array, 0, alloc, 0, len);
                    array = alloc;
                }
            }
            array[count++] = v;
        }

        public int Count => count;

        public V this[int idx] => array[idx];
    }

    public static class RollUtility
    {
        public static R[] RollUp<R, K, V>(this V[] array, Func<V, K> keyer) where R : Roll<K, V>, new()
        {
            if (array == null) return null;

            var list = new ValueList<R>();
            R cur = null; // current
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
            return list.ToArray();
        }
    }
}