using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /brand/
	///
	public class BrandService : WebService
	{
		public BrandService(WebServiceConf wsi) : base(wsi)
		{
			SetXHub<BrandXHub>(false);
		}
	}
}