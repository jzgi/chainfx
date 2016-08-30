using Greatbone.Core;

namespace Greatbone.Sample
{
	public class PostService : WebService
	{
		public PostService(WebServiceContext wsc) : base(wsc)
		{
			AttachXHub<PostXHub>(false);
		}

		public void GetTop(WebContext wc)
		{
		}

		public void Remove(WebContext wc)
		{
		}
	}
}