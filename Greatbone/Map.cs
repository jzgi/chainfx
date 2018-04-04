using System;
using System.Collections;
using System.Collections.Generic;

namespace Greatbone
{
    /// <summary>
    /// An add-only data collection that can act as both a list, a dictionary and/or a two-layered tree.
    /// </summary>
    public class Map<K, V> : IEnumerable<Map<K, V>.Entry>
    {
        int[] buckets;

        protected Entry[] entries;

        int count;

        readonly Predicate<K> toper;

        // indices of top entries
        int[] top;
        int topCount;

        public Map(int capacity = 16, Predicate<K> toper = null)
        {
            // find a least power of 2 that is greater than or equal to capacity
            int size = 8;
            while (size < capacity)
            {
                size <<= 1;
            }
            ReInit(size);
            // init toper 
            if (toper != null)
            {
                this.toper = toper;
                this.top = new int[16];
            }
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

        void AddTop(int idx)
        {
            int len = top.Length;
            if (len <= topCount)
            {
                int[] alloc = new int[len * 2];
                Array.Copy(top, alloc, topCount);
                top = alloc;
            }
            top[topCount++] = idx;
        }

        public int Count => count;

        public Entry At(int idx) => entries[idx];

        public V this[int idx] => entries[idx].value;

        public int TopCount => topCount;

        public Entry TopAt(int idx) => entries[top[idx]];

        public V Top(int idx, out int open, out int close)
        {
            int i = top[idx];
            if (idx < topCount - 1)
            {
                open = i + 1;
                close = top[idx + 1] - 1;
            }
            else
            {
                open = i + 1;
                close = count - 1;
            }
            return entries[i].value;
        }

        public V[] Top()
        {
            V[] arr = new V[topCount];
            for (int i = 0; i < topCount; i++)
            {
                arr[i] = entries[top[i]].value;
            }
            return arr;
        }

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

        public void Add<M>(M v) where M : V, IKeyable<K>
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
            bool istop = toper?.Invoke(key) ?? false; // determine if top or not
            if (istop)
            {
                AddTop(idx);
            }
            entries[idx] = new Entry(code, buckets[buck], key, value, istop);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            return new Enumerator(this);
        }

        //
        // advanced search operations that can be overridden with concurrency constructs

        public V this[K key]
        {
            get
            {
                if (key == null) return default;

                if (TryGet(key, out var val))
                {
                    return val;
                }
                return default;
            }
            set => Add(key, value);
        }

        public V[] All(Predicate<V> cond = null)
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

        public V Find(Predicate<V> cond = null)
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

        public void ForEach(Func<K, V, bool> cond, Action<K, V> hand)
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

        public void ForEachTop(Func<K, V, bool> cond, Action<K, V> hand)
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

            internal readonly bool top;

            internal Entry(int code, int next, K key, V value, bool top)
            {
                this.code = code;
                this.next = next;
                this.key = key;
                this.value = value;
                this.top = top;
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

            public bool IsTop => top;
        }

        public struct Enumerator : IEnumerator<Entry>
        {
            readonly Map<K, V> map;

            int current;

            internal Enumerator(Map<K, V> map)
            {
                this.map = map;
                current = 0;
            }

            public bool MoveNext()
            {
                return ++current < map.Count;
            }

            public void Reset()
            {
                current = 0;
            }

            public Entry Current => map.entries[current];

            object IEnumerator.Current => map.entries[current];

            public void Dispose()
            {
            }
        }
    }
}