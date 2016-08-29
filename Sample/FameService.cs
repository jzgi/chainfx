using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	[Publish("topicname")]
	public class FameService : WebService
	{
		public FameService(WebServiceContext wsc) : base(wsc)
		{
			MountHub<FameXHub>(false);
		}
	}
}