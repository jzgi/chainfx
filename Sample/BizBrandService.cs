using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /brand/
	///
	public class BizBrandService : WebService
	{
		public BizBrandService(WebServiceBuilder builder) : base(builder)
		{
			MountHub<BizBrandUnitHub, Brand>(null);
		}
	}
}