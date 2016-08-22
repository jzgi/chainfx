using Greatbone.Core;

namespace Greatbone.Sample
{
	public class PostXHub : WebXHub<long>
	{
		public PostXHub(WebBuilder builder) : base(builder)
		{
		}

		public void Abc(WebContext wc, long x)
		{
		}
	}
}