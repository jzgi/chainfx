using System.IO;
using System.Threading.Tasks;
using Greatbone.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Greatbone.Sample
{
	public class Program
	{
		///
		/// Microservices
		///
		public static readonly Set<WebService> MicroServices = new Set<WebService>(8)
		{
			new WwwService(new WebServiceContext()
			{
				Key = "www",
				StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "_WWW")
			}),
			new UserService(new WebServiceContext()
			{
				Key = "user",
				StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "_user")
			}),
			new ForumService(new WebServiceContext()
			{
				Key = "forum",
				StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "_forum")
			}),
			new ContentService(new WebServiceContext()
			{
				Key = "content",
				StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "_content")
			}),
			new JobService(new WebServiceContext()
			{
				Key = "job",
				StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "_job")
			})
		};

		///
		/// To dispatch a HTTP context to appropriate target microservice for processing.
		///
		private static async Task Dispatch(HttpContext ctx)
		{
			// the host header to dispatch accordingly
			string host = ctx.Request.Headers["Host"];
			WebService target = null;
			if (host == null)
			{
				target = MicroServices[0];
			}
			else
			{
				for (int i = 0; i < MicroServices.Count; i++)
				{
					WebService svc = MicroServices[i];
					if (svc.IsTarget(host))
					{
						target = svc;
					}
				}
			}
			if (target != null)
			{
				await target.Handle(ctx);
			}
			else
			{
			}
		}

		public static void Main(string[] args)
		{
			bool debug = args.Length > 0 && "debug".Equals(args[0]);

			string addr = debug ? "localhost" : "60.205.104.239";

			using (var host = new WebHostBuilder()
				.UseKestrel()
				.UseUrls("http://" + addr + ":8080/", "http://" + addr + ":9090/")
				.Configure(app => { app.Use(_ => Dispatch); })
				.ConfigureServices((coll) => { coll.AddMemoryCache(); })
				.Build())
			{
				// run the host and lock current thread
				host.Run();
			}
		}
	}
}