using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	[Publish("topicname")]
	public class BizFameSuper : WebSuper
	{
		public BizFameSuper(WebServiceContext wsc) : base(wsc)
		{
			MountHub<BizFameXHub>(false);
		}
	}
}