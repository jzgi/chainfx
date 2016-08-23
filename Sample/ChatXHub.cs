using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public class ChatXHub : WebXHub
	{
		private ConcurrentDictionary<string, WebContext> hanging;

		public ChatXHub(WebServiceContext wsc) : base(wsc)
		{
		}

		public void Get(WebContext wc, string x)
		{
		}

		public void Send(WebContext wc, string x)
		{
		}
	}
}