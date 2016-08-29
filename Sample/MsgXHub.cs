using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public class MsgXHub : WebXHub
	{
		private ConcurrentDictionary<string, WebContext> hanging;

		public MsgXHub(WebServiceContext wsc) : base(wsc)
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