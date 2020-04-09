using System;
using System.Collections;
using System.Collections.Generic;

namespace SkyCloud
{
    /// <summary>
    /// An add-only data collection that can act as both a list, a dictionary and/or a two-layered tree.
    /// </summary>
    public class Map<K, V> : IEnumerable<Map<K, V>.Entry>
    {
        int[] buckets;

        protected Entry[] entries;

        int count;

        // current group head
        int head = -1;

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

        public Entry EntryOf(K key)
        {
            var idx = IndexOf(key);
            if (idx > -1)
            {
                return entries[idx];
            }
            return default;
        }

        public V Find(K key, Predicate<V> cond)
        {
            var idx = IndexOf(key);
            if (idx > -1)
            {
                var ety = entries[idx];
                for (int i = 0; i < ety.Size; i++)
                {
                    var v = ety[i];
                    if (cond(v))
                    {
                        return v;
                    }
                }
            }
            return default;
        }

        public K KeyAt(int idx) => entries[idx].Key;

        public V ValueAt(int idx) => entries[idx].value;

        public V[] GroupOf(K key)
        {
            int idx = IndexOf(key);
            if (idx > -1)
            {
                int tail = entries[idx].tail;
                int ret = tail - idx; // number of returned elements
                V[] arr = new V[ret];
                for (int i = 0; i < ret; i++)
                {
                    arr[i] = entries[idx + 1 + i].value;
                }

                return arr;
            }

            return null;
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
                if (entries[idx].Match(code, key))
                {
                    entries[idx].Add(value);
                    return; // replace the old value
                }

                idx = entries[idx].next; // adjust for next index
            }

            // add a new entry
            idx = count;
            entries[idx] = new Entry(code, buckets[buck], key, value);
            buckets[buck] = idx;
            count++;

            // decide group
            if (value is IGroupKeyable<K> gkeyable)
            {
                // compare to current head
                if (head == -1 || !gkeyable.GroupWith(entries[head].key))
                {
                    head = idx;
                }

                entries[head].tail = idx;
            }
        }

        public bool Contains(K key)
        {
            if (TryGetValue(key, out _))
            {
                return true;
            }

            return false;
        }

        public V this[K key] => TryGetValue(key, out V v) ? v : default;


        public bool TryGetValue(K key, out V value)
        {
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                var e = entries[idx];
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


        public V[] All(Predicate<V> filter)
        {
            var list = new ValueList<V>(16);
            for (int i = 0; i < count; i++)
            {
                var v = entries[i].value;
                if (filter == null || filter(v))
                {
                    list.Add(v);
                }
            }
            return list.ToArray();
        }

        public Map<R, V> All<R>(Func<K, V, bool> filter, Func<V, R> keyer)
        {
            var map = new Map<R, V>(32);
            for (int i = 0; i < count; i++)
            {
                var ety = entries[i];
                for (int k = 0; k < ety.Size; k++)
                {
                    var v = ety[k];
                    if (filter(ety.Key, v))
                    {
                        map.Add(keyer(v), v);
                    }
                }
            }
            return map;
        }

        public V First(Predicate<V> filter)
        {
            for (int i = 0; i < count; i++)
            {
                var v = entries[i].value;
                if (filter == null || filter(v))
                {
                    return v;
                }
            }
            return default;
        }

        public void ForEach(Func<K, V, bool> cond, Action<K, V> handler)
        {
            for (int i = 0; i < count; i++)
            {
                K key = entries[i].key;
                V value = entries[i].value;
                if (cond == null || cond(key, value))
                {
                    handler(entries[i].key, entries[i].value);
                }
            }
        }

        public struct Enumerator : IEnumerator<Entry>
        {
            readonly Map<K, V> map;

            int current;

            internal Enumerator(Map<K, V> map)
            {
                this.map = map;
                current = -1;
            }

            public bool MoveNext()
            {
                return ++current < map.Count;
            }

            public void Reset()
            {
                current = -1;
            }

            public Entry Current => map.entries[current];

            object IEnumerator.Current => map.entries[current];

            public void Dispose()
            {
            }
        }


        /// <summary>
        /// A single entry can hold one ore multiple values, as indicated by size.
        /// </summary>
        public struct Entry : IEnumerable<V>, IKeyable<K>
        {
            readonly int code; // lower 31 bits of hash code

            internal readonly K key; // entry key

            int size; // number of values

            internal V value; // entry value

            V[] array; // extra values

            internal readonly int next; // index of next entry, -1 if last

            internal int tail; // the index of group tail, when this is the head entry

            internal Entry(int code, int next, K key, V value)
            {
                this.code = code;
                this.next = next;
                this.key = key;
                size = 1;
                this.value = value;
                array = null;
                tail = -1;
            }

            internal bool Match(int code, K key)
            {
                return this.code == code && this.key.Equals(key);
            }

            internal void Add(V v)
            {
                if (size == 0)
                {
                    value = v;
                }
                else // add to list
                {
                    // ensure capacity
                    if (array == null)
                    {
                        array = new V[16];
                    }
                    else
                    {
                        int len = array.Length;
                        if (size > len)
                        {
                            var alloc = new V[len * 4];
                            Array.Copy(array, 0, alloc, 0, len);
                            array = alloc;
                        }
                    }
                    array[size - 1] = v;
                }
                size++;
            }

            public IEnumerator<V> GetEnumerator()
            {
                return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(this);
            }

            public override string ToString()
            {
                return key.ToString();
            }

            public K Key => key;

            public V Value => value;

            public int Size => size;

            public V this[int idx] => idx == 0 ? value : array[idx - 1];

            public bool IsHead => tail > -1;

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public struct Enumerator : IEnumerator<V>
            {
                readonly Entry entry;

                int current;

                internal Enumerator(Entry entry)
                {
                    this.entry = entry;
                    current = -1;
                }

                public bool MoveNext()
                {
                    return ++current < entry.size;
                }

                public void Reset()
                {
                    current = -1;
                }

                public V Current => entry[current];

                object IEnumerator.Current => entry[current];

                public void Dispose()
                {
                }
            }
        }
    }
}