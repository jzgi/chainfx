using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			bool debug = args.Length > 0 && "debug".Equals(args[0]);

			int port = 8080;

			DataSourceBuilder dat = new DataSourceBuilder
			{
				Host = "localhost",
				Port = 5432,
				Username = "postgres",
				Password = "Zou###1989"
			};

			var www = new WwwService(new WebServiceBuilder
			{
				Key = "www",
				Host = "60.205.104.239",
				Port = port++,
				DataSource = dat,
				Debug = debug
			});

			var user = new UserService(new WebServiceBuilder
			{
				Key = "user",
				Host = "60.205.104.239",
				Port = port++,
				DataSource = dat,
				Debug = debug
			});

			var forum = new ForumService(new WebServiceBuilder
			{
				Key = "forum",
				Host = "60.205.104.239",
				Port = port,
				DataSource = dat,
				Debug = debug
			});

			var notice = new NoticeService(WebServiceBuilder.Load("job"));

			WebService.Run(www, user, forum, notice);
		}
	}
}