using System;
using System.Collections;

namespace Greatbone.Core
{
    /// <summary>
    /// An add-only data collection that can act as both list and dictionary.
    /// </summary>
    public class Map<K, V> : IEnumerable
    {
        int[] buckets;

        protected Entry[] entries;

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

        void ReInit(int size) // size must be power of 2
        {
            if (entries == null || size > entries.Length) // allocalte new arrays as needed
            {
                buckets = new int[size];
                entries = new Entry[size];
            }
            for (int i = 0; i < buckets.Length; i++) // initialize all buckets to -1
            {
                buckets[i] = -1;
            }
            count = 0;
        }

        public int Count => count;

        public Entry At(int idx) => entries[idx];

        public V this[int idx] => entries[idx].value;

        public int IndexOf(K key)
        {
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                Entry e = entries[idx];
                if (e.Match(code, key))
                {
                    return idx;
                }
                idx = entries[idx].next; // adjust for next index
            }
            return -1;
        }

        public void Clear()
        {
            if (entries != null)
            {
                ReInit(entries.Length);
            }
        }

        public void Add(K key, V value)
        {
            Add(key, value, false);
        }

        public void Add<M>(M v) where M : V, IMappable<K>
        {
            Add(v.Key, v, false);
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

        public bool Contains(K key)
        {
            if (TryGet(key, out _))
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
            value = default;
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        //
        // advanced search operations that can be overridden with concurrency constructs

        public virtual V this[K key]
        {
            get
            {
                if (TryGet(key, out var val))
                {
                    return val;
                }
                return default;
            }
            set => Add(key, value);
        }

        public virtual V[] All(Predicate<V> cond = null)
        {
            Roll<V> roll = new Roll<V>(16);
            for (int i = 0; i < count; i++)
            {
                V v = entries[i].value;
                if (cond == null || cond(v))
                {
                    roll.Add(v);
                }
            }
            return roll.ToArray();
        }

        public virtual V First(Predicate<V> cond = null)
        {
            for (int i = 0; i < count; i++)
            {
                V v = entries[i].value;
                if (cond == null || cond(v))
                {
                    return v;
                }
            }
            return default;
        }

        public virtual void ForEach(Func<K, V, bool> cond, Action<K, V> hand)
        {
            for (int i = 0; i < count; i++)
            {
                K key = entries[i].key;
                V value = entries[i].value;
                if (cond == null || cond(key, value))
                {
                    hand(entries[i].key, entries[i].value);
                }
            }
        }

        public struct Entry
        {
            readonly int code; // lower 31 bits of hash code

            internal readonly K key; // entry key

            internal V value; // entry value

            internal readonly int next; // index of next entry, -1 if last

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