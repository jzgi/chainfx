using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
	public class BizFameXHub : WebXHub<string>
	{
		public BizFameXHub(WebBuilder builder) : base(builder)
		{
		}


		public override void Default(WebContext wc, string x)
		{
			throw new System.NotImplementedException();
		}
	}
}