using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			bool debug = args.Length > 0 && "debug".Equals(args[0]);

			var www = new WwwService(new WebServiceContext()
			{
				Key = "www",
				Host = "60.205.104.239",
				Port = 8080,
				Debug = debug
			});

			var user = new UserService(new WebServiceContext()
			{
				Key = "user",
				Host = "60.205.104.239",
				Port = 8081,
				Debug = debug
			});

			var forum = new ForumService(new WebServiceContext()
			{
				Key = "forum",
				Host = "60.205.104.239",
				Port = 8082,
				Debug = debug
			});

			var biz = new BizService(new WebServiceContext()
			{
				Key = "biz",
				Host = "60.205.104.239",
				Port = 8083,
				Debug = debug
			});

			var job = new JobService(new WebServiceContext()
			{
				Key = "job",
				Host = "60.205.104.239",
				Port = 8084,
				Debug = debug
			});

			WebService.Run(www, user, forum, biz, job);
		}
	}
}