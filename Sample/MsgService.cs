using System.Collections.Concurrent;
using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The forum and commenting servoce.
	///
	public class MsgService : WebService
	{
		private ConcurrentDictionary<string, ChatSession> sessions;

		public MsgService(WebServiceContext wsc) : base(wsc)
		{
			MountHub<MsgXHub>(false);
		}
	}


	struct ChatSession
	{
		private int status;

		private long lasttime;

		WebContext context;
	}
}