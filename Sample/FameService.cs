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
			SetXHub<FameXHub>(false);
		}
	}
}