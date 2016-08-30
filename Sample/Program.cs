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
					@public = "60.205.104.239",
					@private = "localhost:7777",
					peers = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
						"localhost:7783",
					},
					DataSrc = dat,
					debug = true
				}.Load("WWW.json")
			);

			var fame = new FameService(new WebServiceContext
				{
					key = "BIZ",
					@public = "60.205.104.239",
					@private = "60.205.104.239",
					peers = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
						"localhost:7783",
					},
					DataSrc = dat,
					debug = true
				}.Load("BIZ.json")
			);

			var brand = new BrandService(new WebServiceContext
				{
					key = "BIZ",
					@public = "60.205.104.239",
					@private = "60.205.104.239",
					peers = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
						"localhost:7783",
					},
					DataSrc = dat,
					debug = true
				}.Load("BIZ.json")
			);

			var post = new PostService(new WebServiceContext
				{
					key = "POST",
					@public = "60.205.104.239",
					@private = "60.205.104.239",
					peers = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
						"localhost:7783",
					},
					DataSrc = dat,
					debug = true
				}.Load("POST.json")
			);

			var notice = new NoticeService(
				new WebServiceContext
				{
					key = "NOTICE",
					@public = "60.205.104.239",
					@private = "60.205.104.239",
					peers = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
						"localhost:7783",
					},
					DataSrc = dat,
					debug = true
				}.Load("NOTICE.json")
			);

			var user = new UserService(new WebServiceContext
				{
					key = "USER",
					@public = "60.205.104.239",
					@private = "60.205.104.239",
					peers = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
						"localhost:7783",
					},
					DataSrc = dat,
					debug = true
				}.Load("USER.json")
			);

			var msg = new MsgService(new WebServiceContext
				{
					key = "CHAT",
					@public = "60.205.104.239",
					@private = "60.205.104.239",
					peers = new[]
					{
						"localhost:7777",
						"localhost:7778",
						"localhost:7779",
						"localhost:7780",
						"localhost:7781",
						"localhost:7782",
						"localhost:7783",
					},
					DataSrc = dat,
					debug = true
				}.Load("MSG.json")
			);

			WebService.Run(www, fame, brand, post, notice, user, msg);
		}
	}
}