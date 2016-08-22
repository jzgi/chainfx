using Greatbone.Core;

namespace Greatbone.Sample
{
	public class BizService : WebService
	{
		public BizService(WebBuilder builder) : base(builder)
		{
			AddSub<BizFameSuper>("fame", null);

			AddSub<BizBrandSuper>("brand", null);
		}
	}
}