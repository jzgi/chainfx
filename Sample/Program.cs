using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Greatbone.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // the root: /*
            SiteService site = new SiteService();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "RES"))
                .UseUrls("http://localhost:8080/")
                .ConfigureServices((coll) => { coll.AddMemoryCache(); })
                .Configure(app => { app.Use(_ => site.Process); })
                .Build();

            host.Run();


        }
    }
}