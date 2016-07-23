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
                Checker = null
            };

            // create web host by using the kestrel web server
            var host = new WebHostBuilder()
                .UseKestrel()
//                .UseUrls("http://60.205.104.239:8080/")
                .UseUrls("http://localhost:8080/", "http://localhost:9090/")
                .ConfigureServices((coll) => { coll.AddMemoryCache(); })
                .Configure(app => { app.Use(_ => site.Handle); })
                .Build();

            host.Run();
        }
    }
}