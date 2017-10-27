using System;
using System.Collections;

namespace Greatbone.Core
{
    /// <summary>
    /// A dictionary structure that is in placement order.
    /// </summary>
    public class Map<K, V> : IEnumerable
    {
        int[] buckets;

        Entry[] entries;

        int count;

        public Map(int capacity = 16)
        {
            // find a least power of 2 that is greater than or equal to capacity
            int size = 8;
            while (size < capacity)
            {
                size <<= 1;
            }
            ReInit(size);
        }

        void ReInit(int size)
        {
            buckets = new int[size];
            for (int i = 0; i < size; i++)
            {
                buckets[i] = -1; // initialize all buckets to -1
            }
            entries = new Entry[size];
            count = 0;
        }

        public int Count => count;

        public Entry this[int index] => entries[index];

        public V this[K key]
        {
            get
            {
                V val;
                if (TryGet(key, out val))
                {
                    return val;
                }
                return default(V);
            }
            set => Add(key, value);
        }

        public void ForEach(Action<K, V> hand)
        {
            for (int i = 0; i < count; i++)
            {
                hand(entries[i].key, entries[i].value);
            }
        }

        public void ForEach(Func<K, V, bool> test, Action<K, V> hand)
        {
            for (int i = 0; i < count; i++)
            {
                K key = entries[i].key;
                V value = entries[i].value;
                if (test(key, value))
                {
                    hand(entries[i].key, entries[i].value);
                }
            }
        }

        public bool Contains(K key)
        {
            V elem;
            if (TryGet(key, out elem))
            {
                return true;
            }
            return false;
        }

        public bool TryGet(K key, out V value)
        {
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                Entry e = entries[idx];
                if (e.Match(code, key))
                {
                    value = e.value;
                    return true;
                }
                idx = entries[idx].next; // adjust for next index
            }
            value = default(V);
            return false;
        }

        public void Add(K key, V value)
        {
            Add(key, value, false);
        }

        void Add(K key, V value, bool rehash)
        {
            // ensure double-than-needed capacity
            if (!rehash && count >= entries.Length / 2)
            {
                Entry[] old = entries;
                int oldc = count;
                ReInit(entries.Length * 2);
                // re-add old elements
                for (int i = 0; i < oldc; i++)
                {
                    Add(old[i].key, old[i].value, true);
                }
            }

            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                Entry e = entries[idx];
                if (e.Match(code, key))
                {
                    e.value = value;
                    return; // replace the old value
                }
                idx = entries[idx].next; // adjust for next index
            }

            // add a new entry
            idx = count;
            entries[idx] = new Entry(code, buckets[buck], key, value);
            buckets[buck] = idx;
            count++;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public struct Entry
        {
            readonly int code; // lower 31 bits of hash code

            internal readonly K key; // entry key

            internal V value; // entry value

            internal int next; // index of next entry, -1 if last

            internal Entry(int code, int next, K key, V value)
            {
                this.code = code;
                this.next = next;
                this.key = key;
                this.value = value;
            }

            internal bool Match(int code, K key)
            {
                return this.code == code && this.key.Equals(key);
            }

            public override string ToString()
            {
                return value.ToString();
            }

            public K Key => key;

            public V Value => value;
        }
    }
}