using System;
using System.Collections.Generic;

namespace Greatbone
{
    public class Roll<K, V> : IKeyable<K>
    {
        readonly int capacity;

        readonly K key;

        V[] array;

        int count;

        public Roll(K key, int capacity = 16)
        {
            this.capacity = capacity;
            this.key = key;
            array = null;
            count = 0;
        }

        public void Add(V v)
        {
            // ensure capacity
            if (array == null)
            {
                array = new V[capacity];
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

        public K Key => key;

        public int Count => count;

        public V this[int idx] => array[idx];
    }

    public static class RollUtility
    {
        public static Roll<K, V>[] RollUp<K, V>(this V[] array, Func<V, K> keyer)
        {
            List<Roll<K, V>> list = new List<Roll<K, V>>();
            Roll<K, V> roll = null;
            for (int i = 0; i < array?.Length; i++)
            {
                var v = array[i];
                K key = keyer(v);
                if (roll == null)
                {
                    roll = new Roll<K, V>(key);
                    list.Add(roll);
                    roll.Add(v);
                }
                else if (key.Equals(roll.Key))
                {
                    roll.Add(v);
                }
                else // create a new sort
                {
                    roll = new Roll<K, V>(key);
                    list.Add(roll);
                    roll.Add(v);
                }
            }
            return list.ToArray();
        }
    }
}