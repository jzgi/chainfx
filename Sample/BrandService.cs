using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /brand/
	///
	public class BrandService : WebService
	{
		public BrandService(WebServiceConfig cfg) : base(cfg)
		{
			SetXHub<BrandXHub>(false);
		}
	}
}