using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
    ///
    /// The server-side response cache
    ///
    public class ContentCache
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

        public void Add(string url, bool? pub, int maxage, IContent content)
        {
            Item item = new Item(pub, maxage, content, Environment.TickCount);
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

        internal struct Item
        {
            internal readonly bool? pub;

            internal readonly int maxage;

            internal IContent Content { get; }

            internal int Ticks { get; }

            internal Item(bool? pub, int maxage, IContent content, int ticks)
            {
                this.pub = pub;
                this.maxage = maxage;
                Content = content;
                Ticks = ticks;
            }

            internal bool Expired(int now)
            {
                return (Ticks + maxage * 60000) < now;
            }
        }
    }
}