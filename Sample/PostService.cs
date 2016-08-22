using Greatbone.Core;

namespace Greatbone.Sample
{
	public class PostService : WebService
	{
		public PostService(WebBuilder builder) : base(builder)
		{
			MountHub<PostXHub, long>(null);
		}
	}
}