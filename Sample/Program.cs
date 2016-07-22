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
                StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "RES")
            };

            // the root: /*
            SiteService site = new SiteService(wcc)
            {
                Checker = null
            };

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://60.205.104.239:8080/")
                .ConfigureServices((coll) => { coll.AddMemoryCache(); })
                .Configure(app => { app.Use(_ => site.Process); })
                .Build();

            host.Run();
        }
    }
}