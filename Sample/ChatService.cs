using System;
using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{
	/// <summary>The instant messaging servoce.</summary>
	///
	public class ChatService : WebService
	{
		// the ongoing chat sessions, keyed by receiver's ID
		private ConcurrentDictionary<string, Chat> chats;

		public ChatService(WebServiceContext wsc) : base(wsc)
		{
			AttachXHub<ChatXHub>(true);
		}
	}
}