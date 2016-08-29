using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /brand/
	///
	public class BrandService : WebService
	{
		public BrandService(WebServiceContext wsc) : base(wsc)
		{
			MountHub<BrandXHub>(false);
		}
	}
}