using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	///
	[Publish("topicname")]
	public class FameService : WebService
	{
		public FameService(WebServiceBuilder wsc) : base(wsc)
		{
			AttachXHub<FameXHub>(false);
		}
	}
}