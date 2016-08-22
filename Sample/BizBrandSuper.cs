using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /brand/
	///
	public class BizBrandSuper : WebSuper
	{
		public BizBrandSuper(WebBuilder builder) : base(builder)
		{
			MountHub<BizBrandXHub>(null);
		}
	}
}