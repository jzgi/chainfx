using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /brand/
	///
	public class BizBrandSuper : WebSuper
	{
		public BizBrandSuper(WebServiceBuilder builder) : base(builder)
		{
			MountHub<BizBrandXHub>(null);
		}
	}
}