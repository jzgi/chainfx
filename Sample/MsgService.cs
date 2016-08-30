using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
	/// <summary>The instant messaging servoce.</summary>
	///
	public class MsgService : WebService
	{
		// the ongoing chat sessions, keyed by receiver's ID
		private ConcurrentDictionary<string, Chat> chats;

		public MsgService(WebServiceContext wsc) : base(wsc)
		{
			AttachXHub<MsgXHub>(true);
		}
	}


	/// <summary>Represent a chat session.</summary>
	///
	struct Chat
	{
		private int status;

		private List<Message> msgs;

		private long lasttime;

		WebContext wctx;

		internal void Put(string msg)
		{
			msgs.Add(new Message());
		}
	}

	struct Message
	{
		DateTime time;

		string text;
	}
}