using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Greatbone.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // the root: /*
            OpHub root = new OpHub();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "RES"))
                .UseUrls("http://localhost:8080/")
                .Configure(app => { app.Use(_ => root.Process); })
                .Build();

            host.Run();
        }
    }
}