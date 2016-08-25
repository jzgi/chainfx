using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			bool debug = args.Length > 0 && "debug".Equals(args[0]);

			int port = 8080;

			DataSrcBuilder dat = new DataSrcBuilder
			{
				host = "localhost",
				port = 5432,
				username = "postgres",
				password = "Zou###1989"
			};

			var www = new WwwService(new WebServiceContext
				{
					key = "www",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("www.json")
			);

			var biz = new BizService(new WebServiceContext
				{
					key = "biz",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("biz.json")
			);

			var post = new PostService(new WebServiceContext
				{
					key = "post",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("post.json")
			);

			var user = new UserService(new WebServiceContext
				{
					key = "user",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("user.json")
			);

			var chat = new ChatService(new WebServiceContext
				{
					key = "chat",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("chat.json")
			);

			var notice = new NoticeService(new WebServiceContext().Load("notice.json"));

			WebService.Run(www, biz, post, user, chat, notice);
		}
	}
}