using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
	public class FameXHub : WebXHub
	{
		public FameXHub(WebServiceContext wsc) : base(wsc)
		{
		}


		public override void Default(WebContext wc, string x)
		{
			throw new System.NotImplementedException();
		}
	}
}