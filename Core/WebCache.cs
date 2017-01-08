using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
    /// 
    /// A response cache in service.
    /// 
    public class WebCache
    {
        // keyed by target uri
        readonly ConcurrentDictionary<string, Entry> entries;

        Action<int>[] handlers;

        internal WebCache(int concurrency, int capcity)
        {
            entries = new ConcurrentDictionary<string, Entry>(concurrency, capcity);
            handlers = new Action<int>[8];
        }

        public void SetHandler(int ordinal, Action<int> a)
        {
            handlers[ordinal] = a;
        }

        public void Add(string target, int seconds, IContent content)
        {
            Entry e = new Entry(seconds, content, Environment.TickCount);
            entries.AddOrUpdate(target, e, (k, v) => e.Merge(v));
        }

        public void Remove(string target)
        {
            return;
        }

        public void Clear()
        {
            return;
        }

        internal void Clean()
        {
            int now = Environment.TickCount;

            // a single loop to clean up expired items
            using (var enm = entries.GetEnumerator())
            {
                while (enm.MoveNext())
                {
                    Entry e = enm.Current.Value;
                    if (e.IfExpired(now))
                    {
                        Entry old;
                        entries.TryRemove(enm.Current.Key, out old);
                    }
                }
            }
        }

        internal bool TryGetContent(string target, out IContent v)
        {
            Entry e;
            if (entries.TryGetValue(target, out e))
            {
                e.Increment();
                v = e.content;
                return true;
            }
            v = null;
            return false;
        }

        class Entry
        {
            // ticks of expiration
            int expiry;

            // can be null
            internal IContent content;

            int ticks;

            int hits;

            internal Entry(int expiry, IContent content, int ticks)
            {
                this.expiry = expiry;
                this.content = content;
                this.ticks = ticks;
            }

            internal void Increment()
            {
                Interlocked.Increment(ref hits);
            }

            internal int Counter => hits;

            internal bool IfExpired(int now)
            {
                return (ticks + expiry * 60000) < now;
            }

            internal Entry Merge(Entry e)
            {
                expiry = e.expiry;
                hits += e.hits;
                content = e.content;
                return this;
            }
        }
    }
}