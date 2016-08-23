using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public class ChatXHub : WebXHub
	{
		private ConcurrentDictionary<string, WebContext> hanging;

		public ChatXHub(WebBuilder builder) : base(builder)
		{
		}

		[Allow("@")]
		public void Get(WebContext wc, string x)
		{

		}

		[Allow("*")]
		public void Send(WebContext wc, string x)
		{

		}
	}
}