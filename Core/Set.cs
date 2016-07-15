using System.Collections;
using System.Collections.Generic;

namespace Greatbone.Core
{

    /// <summary>
    /// A member of set that is identified by character key, for mapping with components in HTTP request
    /// </summary>
    /// <remarks>URL and parameters keep case-sensitive semantic</remarks>
    public interface IMember
    {
        string Key { get; }
    }


    /// <summary>
    /// An addition-only collection of elements with character keys. The members are placed in the addition order. 
    /// </summary>
    public class Set<TM> : ICollection<TM> where TM : IMember
    {

        int[] _buckets;

        Entry[] _entries;

        // number of entries
        int _count;

        public Set(int capacity)
        {
            // find a least power of 2 that is greater than or equal to capacity
            int size = 1;
            while (size < capacity) { size <<= 1; }

            ReInitialize(size);
        }

      private void ReInitialize(int size)
        {
            _buckets = new int[size];
            for (int i = 0; i < size; i++)
            {
                _buckets[i] = -1; // initialize all buckets to -1
            }
            _entries = new Entry[size];
            _count = 0;
        }

        public TM this[int index] => _entries[index].member;

        public TM this[string key]
        {
            get
            {
                TM mbr;
                if (TryGet(key, out mbr))
                {
                    return mbr;
                }
                return default(TM);
            }
        }

        public bool TryGet(string key, out TM member)
        {
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % _buckets.Length; // target bucket
            int idx = _buckets[buck];
            while (idx != -1)
            {
                Entry e = _entries[idx];
                if (e.Match(code, key))
                {
                    member = e.member;
                    return true;
                }
                idx = _entries[idx].next; // adjust for next index
            }
            member = default(TM);
            return false;
        }

        public int Count => _count;

        public void Add(TM member)
        {
            Add(member, false);
        }

        public void Add(TM member, bool rehash)
        {
            // ensure double-than-needed capacity
            if (!rehash && _count >= _entries.Length / 2)
            {
                Entry[] old = _entries;
                int oldcount = _count;
                ReInitialize(_entries.Length * 2);
                // re-add old elements
                for (int i = 0; i < oldcount; i++)
                {
                    Add(old[i].member, true);
                }
            }

            string key = member.Key;
            int code = key.GetHashCode() & 0x7fffffff;
            int buck = code % _buckets.Length; // target bucket
            int idx = _buckets[buck];
            while (idx != -1)
            {
                Entry e = _entries[idx];
                if (e.Match(code, key))
                {
                    e.member = member; return; // replace the old value
                }
                idx = _entries[idx].next;  // adjust for next index
            }

            //
            // add a new entry

            idx = _count;
            _entries[idx] = new Entry(code, _buckets[buck], key, member);
            _buckets[buck] = idx;
            _count++;
        }

        public void Clear()
        {
        }

        public bool Contains(TM item)
        {
            TM mbr;
            return TryGet(item.Key, out mbr);
        }

        public void CopyTo(TM[] array, int arrayIndex)
        {
        }

        public bool IsReadOnly => true;

        public bool Remove(TM item)
        {
            return false;
        }

        public IEnumerator<TM> GetEnumerator()
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

            internal TM member; // entry value

            internal int next;  // index of next entry, -1 if last

            internal Entry(int code, int next, string key, TM member)
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