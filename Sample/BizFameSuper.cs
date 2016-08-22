using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	[Publish("topicname")]
	public class BizFameSuper : WebSuper
	{
		public BizFameSuper(WebBuilder builder) : base(builder)
		{
			MountHub<BizFameXHub, string>(null);
		}
	}
}