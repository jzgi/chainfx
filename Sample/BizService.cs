using Greatbone.Core;

namespace Greatbone.Sample
{
	public class BizService : WebService
	{
		public BizService(WebServiceBuilder builder) : base(builder)
		{
			AddSub<BizFameService>("fame", null);

			AddSub<BizBrandService>("brand", null);
		}
	}
}