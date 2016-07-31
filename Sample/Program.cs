using System.IO;
using Greatbone.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Greatbone.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// create the creation context for the root service
			WebCreationContext wcc = new WebCreationContext
			{
				Key = "site",
				StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "RES")
			};

			// create the site service without access check
			SiteService site = new SiteService(wcc)
			{
//				Checker = null
			};

			bool debug = args.Length > 0 && "debug".Equals(args[0]);

			string addr = debug ? "localhost" : "60.205.104.239";
			// create web host by using the kestrel web server
			WebServiceHost host = (WebServiceHost) new WebServiceHostBuilder()
				.UseKestrel()
				.UseUrls("http://" + addr + ":8080/", "http://" + addr + ":9090/")
				.ConfigureServices((coll) => { coll.AddMemoryCache(); })
//				.Configure(app => { app.Use(_ => site.Handle); })
				.Build();

			host.AddService<DirectoryService>("dir", null);

			host.Run();
		}
	}
}