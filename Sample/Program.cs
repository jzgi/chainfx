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

			var www = new WwwService(new WebBuilder
			{
				Key = "www",
				Host = "60.205.104.239",
				Port = port++,
				DataSource = dat,
				Debug = debug
			});

			var biz = new BizService(new WebBuilder
			{
				Key = "biz",
				Host = "60.205.104.239",
				Port = port++,
				DataSource = dat,
				Debug = debug
			});

			var post = new PostService(new WebBuilder
			{
				Key = "post",
				Host = "60.205.104.239",
				Port = port++,
				DataSource = dat,
				Debug = debug
			});

			var user = new UserService(new WebBuilder
			{
				Key = "user",
				Host = "60.205.104.239",
				Port = port++,
				DataSource = dat,
				Debug = debug
			});

			var chat = new ChatService(new WebBuilder
			{
				Key = "chat",
				Host = "60.205.104.239",
				Port = port,
				DataSource = dat,
				Debug = debug
			});

			var notice = new NoticeService(WebBuilder.Load("job")
			);

			WebService.Run(www, biz, post, user, chat, notice);
		}
	}
}