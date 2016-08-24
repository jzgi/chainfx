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
					key = "WWW",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("WWW.json")
			);

			var biz = new BizService(new WebServiceContext
				{
					key = "BIZ",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("BIZ.json")
			);

			var post = new PostService(new WebServiceContext
				{
					key = "POST",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("POST.json")
			);

			var user = new UserService(new WebServiceContext
				{
					key = "USER",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("USER.json")
			);

			var chat = new ChatService(new WebServiceContext
				{
					key = "CHAT",
					host = "60.205.104.239",
					datasrc = dat,
					debug = debug
				}.Load("CHAT.json")
			);

			var notice = new NoticeService(new WebServiceContext().Load("NOTICE.json"));

			WebService.Run(www, biz, post, user, chat, notice);
		}
	}
}