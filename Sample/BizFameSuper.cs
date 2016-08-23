using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	[Publish("topicname")]
	public class BizFameSuper : WebSuper
	{
		public BizFameSuper(WebServiceBuilder builder) : base(builder)
		{
			MountHub<BizFameXHub>(null);
		}
	}
}