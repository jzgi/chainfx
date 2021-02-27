namespace SkyChain.Db
{
    /// <summary>
    /// A classifier that realizes graph-based mechine learning.
    /// </summary>
    public class MlGraph<P, R> where P : struct, IFeature<P> where R : struct, IOutcome<R>
    {
        private MlNode[] nodes;
        
        
        
        //
        // key & value mapping

        int[] buckets;

        Entry[] entries;

        int count;


        public MlGraph(int capacity)
        {
            // find a least power of 2 that is greater than or equal to capacity
            int num = 1024;
            while (num < capacity)
            {
                num <<= 1;
            }
            ReInit(num);
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

        public P PathAt(int idx) => entries[idx].path;

        public R ResultAt(int idx) => entries[idx].result;


        public int IndexOf(P path)
        {
            int code = path.GetHashCode();
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                var e = entries[idx];
                if (e.Match(code, path))
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

        public void Add(P path, R result)
        {
            Add(path, result, false);
        }

        void Add(P path, R result, bool rehash)
        {
            // ensure double-than-needed capacity
            if (!rehash && count >= entries.Length / 2)
            {
                var old = entries;
                int oldc = count;
                ReInit(entries.Length * 2);
                // re-add old elements
                for (int i = 0; i < oldc; i++)
                {
                    Add(old[i].path, old[i].result, true);
                }
            }

            int code = path.GetHashCode();
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                if (entries[idx].Match(code, path))
                {
                    entries[idx].Add(result);
                    return; // merge
                }
                idx = entries[idx].next; // adjust for next index
            }

            // add a new entry
            idx = count;
            entries[idx] = new Entry(code, buckets[buck], path, result);
            buckets[buck] = idx;
            count++;
        }

        public R this[P key] => TryGetValue(key, out R v) ? v : default;


        public bool TryGetValue(P key, out R value)
        {
            int code = key.GetHashCode();
            int buck = code % buckets.Length; // target bucket
            int idx = buckets[buck];
            while (idx != -1)
            {
                var e = entries[idx];
                if (e.Match(code, key))
                {
                    value = e.result;
                    return true;
                }
                idx = entries[idx].next; // adjust for next index
            }
            value = default;
            return false;
        }


        internal struct Entry
        {
            readonly int code; // lower 31 bits of hash code

            internal P path;

            internal R result;

            internal readonly int next; // index of next entry, -1 if last

            internal Entry(int code, int next, P path, R result)
            {
                this.code = code;
                this.next = next;
                this.path = path;
                this.result = result;
            }

            internal void Add(R @new)
            {
                result.OnAdd(@new);
            }

            internal bool Match(int code, P path)
            {
                return this.code == code;
            }

            public P Path => path;

            public R Result => result;

            public override string ToString()
            {
                return path.ToString();
            }
        }
    }
}