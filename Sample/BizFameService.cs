using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	[Publish("topicname")]
	public class BizFameService : WebService
	{
		public BizFameService(WebServiceBuilder builder) : base(builder)
		{
			MountHub<BizFameUnitHub, Fame>(null);
		}
	}
}