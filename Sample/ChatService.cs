using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
	/// <summary>The chat servoce.</summary>
	///
	public class ChatService : AbsService
	{
		// the ongoing chat sessions, keyed by receiver's ID
		private ConcurrentDictionary<string, Wrap> chats = new ConcurrentDictionary<string, Wrap>();

		public ChatService(WebConfig cfg) : base(cfg)
		{
			SetVarHub<ChatVarHub>(true);
		}

	    public void Get(WebContext wc)
	    {
	        Wrap w = new Wrap()
	        {
	            key = "123",
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

	        internal TaskCompletionSource<int> tcs;

	    }

		struct Session
		{
			internal TaskCompletionSource<string> com;
		}

	}

}