using Greatbone.Core;

namespace Greatbone.Sample
{
	public class BizService : WebService
	{
		public BizService(WebServiceContext wsc) : base(wsc)
		{
			AddSub<BizFameSuper>("fame", null);

			AddSub<BizBrandSuper>("brand", null);
		}
	}
}