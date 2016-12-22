using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
    /// 
    /// The server-side response cache in particular service.
    /// 
    class ResponseCache
    {
        readonly ConcurrentDictionary<string, Entry> entries;

        readonly Thread cleaner;

        volatile bool stop;

        internal ResponseCache(int concurrency, int capcity)
        {
            // create the url-to-item dictionary
            entries = new ConcurrentDictionary<string, Entry>(concurrency, capcity);

            // create and start the cleaner thread
            cleaner = new Thread(Clean);
            cleaner.Start();
        }

        internal void Stop()
        {
            stop = true;
        }

        internal void Add(string target, int maxage, IContent content)
        {
            Entry e = new Entry(maxage, content, Environment.TickCount);
            entries.AddOrUpdate(target, e, (k, v) => e.Merge(v));
        }

        internal void Clean()
        {
            while (!stop)
            {
                Thread.Sleep(1000);

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
        }

        internal bool TryGetContent(string target, out IContent v)
        {
            Entry e;
            if (entries.TryGetValue(target, out e))
            {
                e.Inc();
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
            IContent content;

            int ticks;

            int counter;

            internal Entry(int expiry, IContent content, int ticks)
            {
                this.expiry = expiry;
                this.content = content;
                this.ticks = ticks;
            }

            internal void Inc()
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