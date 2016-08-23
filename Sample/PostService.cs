using Greatbone.Core;

namespace Greatbone.Sample
{
	public class PostService : WebService
	{
		public PostService(WebBuilder builder) : base(builder)
		{
			MountHub<PostXHub>(null);
		}

		[Allow("*")]
		public void GetTop(WebContext wc)
		{

		}

		[Allow("cm")]
		public void Remove(WebContext wc)
		{

		}
	}
}