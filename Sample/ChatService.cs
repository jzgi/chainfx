using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
	/// <summary>The chat servoce.</summary>
	///
	public class ChatService : WebService
	{
		// the ongoing chat sessions, keyed by receiver's ID
		private ConcurrentDictionary<string, Wrap> chats = new ConcurrentDictionary<string, Wrap>();

		public ChatService(WebServiceConfig cfg) : base(cfg)
		{
			SetXHub<ChatXHub>(true);
		}

	    public void Foo(WebContext wc)
	    {
	        Wrap w = new Wrap()
	        {
	            key = "123",
	            ctx = wc,
	            tcs = new TaskCompletionSource<int>()
	        };

	        chats.TryAdd("123", w);
	    }

	    public void Bar(WebContext wc)
	    {


	    }

	    internal struct Wrap
	    {
	        internal string key;

	        internal WebContext ctx;

	        internal TaskCompletionSource<int> tcs;

	    }

	}
}