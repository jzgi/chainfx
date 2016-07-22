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
            // the static files
            WebCreationContext context = new WebCreationContext
            {
                StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "RES")
            };

            // the root: /*
            SiteService site = new SiteService(context);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "RES"))
                .UseUrls("http://60.205.104.239:8080/")
                .ConfigureServices((coll) => { coll.AddMemoryCache(); })
                .Configure(app => { app.Use(_ => site.Process); })
                .Build();

            host.Run();
        }
    }
}