using System.Collections;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A member of set that is identified by character key, for mapping with components in HTTP request
    /// </summary>
    /// <remarks>URL and parameters keep case-sensitive semantic</remarks>
    public interface IKeyed
    {
        string Key { get; }
    }


    /// <summary>
    /// An addition-only collection of elements with character keys. The members are placed in the addition order.
    /// </summary>
    public class Roll<T> : ICollection<T> where T : IKeyed
    {
        int[] buckets;

        Entry[] entries;

        // number of entries
        int count;

        public Roll(int capacity)
        {
            // find a least power of 2 that is greater than or equal to capacity
            int size = 1;
            while (size < capacity)
            {
                size <<= 1;
            }

            ReInitialize(size);
        }

        private void ReInitialize(int size)
        {
            buckets = new int[size];
            for (int i = 0; i < size; i++)
            {
                buckets[i] = -1; // initialize all buckets to -1
            }
            entries = new Entry[size];
            count = 0;
        }

        public T this[int index] => entries[index].member;

        public T this[string key]
        {
            get
            {
                T mbr;
                if (TryGet(key, out mbr))
                {
                    return mbr;
                }
                return default(T);
            }
        }

        public bool TryGet(string key, out T member)
        {
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                Entry e = entries[idx];
                if (e.Match(code, key))
                {
                    member = e.member;
                    return true;
                }
                idx = entries[idx].next; // adjust for next index
            }
            member = default(T);
            return false;
        }

        public int Count => count;

        public void Add(T member)
        {
            Add(member, false);
        }

        public void Add(T member, bool rehash)
        {
            // ensure double-than-needed capacity
            if (!rehash && count >= entries.Length / 2)
            {
                Entry[] old = entries;
                int oldcount = count;
                ReInitialize(entries.Length * 2);
                // re-add old elements
                for (int i = 0; i < oldcount; i++)
                {
                    Add(old[i].member, true);
                }
            }

            string key = member.Key;
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                Entry e = entries[idx];
                if (e.Match(code, key))
                {
                    e.member = member;
                    return; // replace the old value
                }
                idx = entries[idx].next; // adjust for next index
            }

            //
            // add a new entry

            idx = count;
            entries[idx] = new Entry(code, buckets[buck], key, member);
            buckets[buck] = idx;
            count++;
        }

        public void Clear()
        {
        }

        public bool Contains(T item)
        {
            T mbr;
            return TryGet(item.Key, out mbr);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
        }

        public bool IsReadOnly => true;

        public bool Remove(T item)
        {
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        public struct Entry
        {
            internal int code; // lower 31 bits of hash code

            internal string key; // entry key

            internal T member; // entry value

            internal int next; // index of next entry, -1 if last

            internal Entry(int code, int next, string key, T member)
            {
                this.code = code;
                this.next = next;
                this.key = key;
                this.member = member;
            }

            internal bool Match(int code, string key)
            {
                return this.code == code && this.key.Equals(key);
            }

            public override string ToString()
            {
                return member.ToString();
            }
        }
    }
}