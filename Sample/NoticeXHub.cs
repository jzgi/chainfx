using Greatbone.Core;

namespace Greatbone.Sample
{
	public class NoticeXHub : WebXHub<int>
	{
		public NoticeXHub(WebBuilder builder) : base(builder)
		{
		}

		public void Abc(WebContext wc, int x)
		{
		}
	}
}