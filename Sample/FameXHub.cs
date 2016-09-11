using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
	public class FameXHub : WebXHub
	{
		public FameXHub(WebSubConfig cfg) : base(cfg)
		{
		}


		public override void Default(WebContext wc, string x)
		{
			throw new System.NotImplementedException();
		}
	}
}