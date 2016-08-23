using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
	public class BizFameXHub : WebXHub
	{
		public BizFameXHub(WebServiceBuilder builder) : base(builder)
		{
		}


		public override void Default(WebContext wc, string x)
		{
			throw new System.NotImplementedException();
		}
	}
}