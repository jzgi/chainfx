using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
    /// <summary>
    /// The server-side content cache for a particular service.
    /// </summary>
    class ContentCache
    {
        readonly ConcurrentDictionary<string, Item> items;

        readonly Thread cleaner;

        volatile bool stop;

        internal ContentCache(int concurrency, int capcity)
        {
            // create the url-to-item dictionary
            items = new ConcurrentDictionary<string, Item>(concurrency, capcity);

            // create and start the cleaner thread
            cleaner = new Thread(Clean);
            cleaner.Start();
        }

        internal void ToStop()
        {
            stop = true;
        }

        internal void Add(string url, int maxage, IContent content)
        {
            Item item = new Item(maxage, content, Environment.TickCount);
            items.TryAdd(url, item);
        }

        internal void Clean()
        {
            while (!stop)
            {
                Thread.Sleep(1000 * 12);

                int now = Environment.TickCount;

                // a single loop to clean up expired items
                using (var e = items.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        Item item = e.Current.Value;
                        if (item.Expired(now))
                        {
                            Item old;
                            items.TryRemove(e.Current.Key, out old);
                        }
                    }
                }
            }

        }

        internal bool TryGetContent(string target, out IContent v)
        {
            Item itm;
            if (items.TryGetValue(target, out itm))
            {
                v = itm.Content;
                return true;
            }
            v = null;
            return false;
        }

        struct Item
        {
            // ticks of expiration
            internal readonly int expiry;

            // can be null
            internal IContent Content { get; }

            internal int Ticks { get; }

            internal Item(int expiry, IContent content, int ticks)
            {
                this.expiry = expiry;
                Content = content;
                Ticks = ticks;
            }

            internal bool Expired(int now)
            {
                return (Ticks + expiry * 60000) < now;
            }
        }
    }
}