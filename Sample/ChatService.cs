using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The forum and commenting servoce.
	///
	public class ChatService : WebService
	{
		private ConcurrentDictionary<string, ChatSession> sessions;

		public ChatService(WebServiceBuilder builder) : base(builder)
		{
			MountHub<ChatXHub>(null);
		}
	}


	struct ChatSession
	{
		private int status;

		private long lasttime;

		WebContext context;
	}
}