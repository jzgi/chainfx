using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
    /// 
    /// A response cache in service.
    /// 
    class ResponseCache
    {
        // keyed by target uri
        readonly ConcurrentDictionary<string, Entry> entries;

        internal ResponseCache(int concurrency, int capcity)
        {
            entries = new ConcurrentDictionary<string, Entry>(concurrency, capcity);
        }

        internal void Add(string target, int maxage, IContent content)
        {
            Entry e = new Entry(maxage, content, Environment.TickCount);
            entries.AddOrUpdate(target, e, (k, v) => e.Merge(v));
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

            int counter;

            internal Entry(int expiry, IContent content, int ticks)
            {
                this.expiry = expiry;
                this.content = content;
                this.ticks = ticks;
            }

            internal void Increment()
            {
                Interlocked.Increment(ref counter);
            }

            internal int Counter => counter;

            internal bool IfExpired(int now)
            {
                return (ticks + expiry * 60000) < now;
            }

            internal Entry Merge(Entry e)
            {
                expiry = e.expiry;
                counter += e.counter;
                content = e.content;
                return this;
            }
        }
    }
}