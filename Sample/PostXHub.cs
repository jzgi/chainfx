using Greatbone.Core;

namespace Greatbone.Sample
{
	public class PostXHub : WebXHub
	{
		public PostXHub(WebBuilder builder) : base(builder)
		{
		}

		public void Get(WebContext wc, long x)
		{
		}

		[Allow("@")]
		public void Delete(WebContext wc, long x)
		{
		}
	}
}