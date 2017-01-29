using System;
using System.Collections;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// An addition-only and -ordered dictionary.
    ///
    public class Roll<E> : IEnumerable where E : IKeyed<string>
    {
        int[] buckets;

        Entry[] entries;

        int count;


        public Roll(int capacity)
        {
            // find a least power of 2 that is greater than or equal to capacity
            int size = 1;
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

        public E this[int index] => entries[index].element;

        public int Count => count;

        public E this[string key]
        {
            get
            {
                E elem;
                if (TryGet(key, out elem))
                {
                    return elem;
                }
                return default(E);
            }
            set
            {
                Add(value);
            }
        }

        public E Any(string prefix)
        {
            for (int i = 0; i < count; i++)
            {
                E elem = entries[i].element;
                if (elem.Key.StartsWith(prefix)) return elem;
            }
            return default(E);
        }

        public E Any(Predicate<E> match)
        {
            for (int i = 0; i < count; i++)
            {
                E elem = entries[i].element;
                if (match(elem)) return elem;
            }
            return default(E);
        }

        public E[] All(string prefix)
        {
            List<E> lst = null;
            for (int i = 0; i < count; i++)
            {
                E elem = entries[i].element;
                if (elem.Key.StartsWith(prefix))
                {
                    if (lst == null) lst = new List<E>(8);
                    lst.Add(elem);
                }
            }
            return lst?.ToArray();
        }

        public E[] All(Predicate<E> match)
        {
            List<E> lst = null;
            for (int i = 0; i < count; i++)
            {
                E elem = entries[i].element;
                if (match(elem))
                {
                    if (lst == null) lst = new List<E>(8);
                    lst.Add(elem);
                }
            }
            return lst?.ToArray();
        }

        public void ForEach(Action<E> a)
        {
            for (int i = 0; i < count; i++)
            {
                E elem = entries[i].element;
                a(elem);
            }
        }

        public void ForEach(string prefix, Action<E> a)
        {
            for (int i = 0; i < count; i++)
            {
                E elem = entries[i].element;
                if (elem.Key.StartsWith(prefix))
                {
                    a(elem);
                }
            }
        }

        public void ForEach(Predicate<E> match, Action<E> a)
        {
            for (int i = 0; i < count; i++)
            {
                E elem = entries[i].element;
                if (match(elem))
                {
                    a(elem);
                }
            }
        }

        public bool Contains(string key)
        {
            E elem;
            if (TryGet(key, out elem))
            {
                return true;
            }
            return false;
        }

        public bool TryGet(string key, out E elem)
        {
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                Entry e = entries[idx];
                if (e.Match(code, key))
                {
                    elem = e.element;
                    return true;
                }
                idx = entries[idx].next; // adjust for next index
            }
            elem = default(E);
            return false;
        }

        public void Add(E elem)
        {
            Add(elem, false);
        }

        void Add(E elem, bool rehash)
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
                    Add(old[i].element, true);
                }
            }

            string key = elem.Key;
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                Entry e = entries[idx];
                if (e.Match(code, key))
                {
                    e.element = elem;
                    return; // replace the old value
                }
                idx = entries[idx].next; // adjust for next index
            }

            // add a new entry
            idx = count;
            entries[idx] = new Entry(code, buckets[buck], key, elem);
            buckets[buck] = idx;
            count++;
        }

        public IEnumerator GetEnumerator() => null;


        struct Entry
        {
            readonly int code; // lower 31 bits of hash code

            readonly string key; // entry key

            internal E element; // entry value

            internal int next; // index of next entry, -1 if last

            internal Entry(int code, int next, string key, E elem)
            {
                this.code = code;
                this.next = next;
                this.key = key;
                this.element = elem;
            }

            internal bool Match(int code, string key)
            {
                return this.code == code && this.key.Equals(key);
            }

            public override string ToString()
            {
                return element.ToString();
            }
        }
    }
}