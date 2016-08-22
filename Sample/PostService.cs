using Greatbone.Core;

namespace Greatbone.Sample
{
	public class PostService : WebService
	{
		public PostService(WebServiceBuilder builder) : base(builder)
		{
			MountHub<PostUnitHub, Post>(null);
		}
	}
}