using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /brand/
	///
	public class BizBrandSuper : WebSuper
	{
		public BizBrandSuper(WebServiceContext wsc) : base(wsc)
		{
			MountHub<BizBrandXHub>(null);
		}
	}
}