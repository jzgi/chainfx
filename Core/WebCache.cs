using System.Collections.Concurrent;

namespace Greatbone.Core
{
	public class WebCache
	{
		private readonly ConcurrentDictionary<string, Item> _items;


		public WebCache(int concurrency, int capcity)
		{
			_items = new ConcurrentDictionary<string, Item>(concurrency, capcity);

		}

		internal struct Item
		{
		}
	}
}