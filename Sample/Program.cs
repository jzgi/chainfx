using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			bool debug = args.Length > 0 && "debug".Equals(args[0]);

			var www = new WwwService(new WebServiceBuilder()
			{
				Key = "www",
				Host = "60.205.104.239",
				Port = 8080,
				Debug = debug
			});

			var user = new UserService(new WebServiceBuilder()
			{
				Key = "user",
				Host = "60.205.104.239",
				Port = 8081,
				Debug = debug
			});

			var forum = new ForumService(new WebServiceBuilder()
			{
				Key = "forum",
				Host = "60.205.104.239",
				Port = 8082,
				Debug = debug
			});

			var biz = new BizService(new WebServiceBuilder()
			{
				Key = "biz",
				Host = "60.205.104.239",
				Port = 8083,
				Debug = debug
			});

			var job = new JobService(new WebServiceBuilder()
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