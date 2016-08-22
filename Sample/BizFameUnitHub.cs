using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
	public class BizFameUnitHub : WebUnitHub<Fame>
	{
		public BizFameUnitHub(WebServiceBuilder builder) : base(builder)
		{
		}


		public override void Default(WebContext wc, Fame unit)
		{
			throw new System.NotImplementedException();
		}
	}
}