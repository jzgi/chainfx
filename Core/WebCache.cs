using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Greatbone.Core
{
	///
	/// The server-side response cache
	///
	public class WebCache
	{
		private readonly ConcurrentDictionary<string, Item> items;

		private readonly Thread cleaner;

		private volatile bool stop;

		public WebCache(int concurrency, int capcity)
		{
			// create the url-to-item dictionary
			items = new ConcurrentDictionary<string, Item>(concurrency, capcity);

			// create and start the cleaner thread
			cleaner = new Thread(Clean);
			cleaner.Start();
		}

		public void Add(string url, CachePolicy policy, IContent content)
		{
			Item item = new Item(policy, content, Environment.TickCount);
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
			internal CachePolicy Policy { get; }

			internal IContent Content { get; }

			//
			internal int Ticks;

			internal Item(CachePolicy policy, IContent content, int ticks)
			{
				Policy = policy;
				Content = content;
				Ticks = ticks;
			}

			internal bool Expired(int now)
			{
				return (Ticks + Policy.MaxAge * 60000) < now;
			}
		}
	}
}