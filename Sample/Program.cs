using Greatbone.Core;

namespace Greatbone.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
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
					address = "60.205.104.239",
					cluster = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
					},
					datasrc = dat,
					debug = true
				}.Load("WWW.json")
			);

			var biz = new BrandService(new WebServiceContext
				{
					key = "BIZ",
					address = "60.205.104.239",
					cluster = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
					},
					datasrc = dat,
					debug = true
				}.Load("BIZ.json")
			);

			var fame = new FameService(new WebServiceContext
				{
					key = "BIZ",
					address = "60.205.104.239",
					cluster = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
					},
					datasrc = dat,
					debug = true
				}.Load("BIZ.json")
			);

			var post = new PostService(new WebServiceContext
				{
					key = "POST",
					address = "60.205.104.239",
					cluster = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
					},
					datasrc = dat,
					debug = true
				}.Load("POST.json")
			);

			var user = new UserService(new WebServiceContext
				{
					key = "USER",
					address = "60.205.104.239",
					cluster = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
					},
					datasrc = dat,
					debug = true
				}.Load("USER.json")
			);

			var chat = new MsgService(new WebServiceContext
				{
					key = "CHAT",
					address = "60.205.104.239",
					cluster = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
					},
					datasrc = dat,
					debug = true
				}.Load("CHAT.json")
			);

			var notice = new NoticeService(new WebServiceContext().Load("notice.json"));

			WebService.Run(www, fame, post, user, chat, notice);
		}
	}
}