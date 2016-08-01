using System;
using System.ComponentModel.DataAnnotations;
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
            new DirectoryService(new WebServiceContext()
            {
                Key = "dir",
                StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "DIR")
            }),
            new SiteService(new WebServiceContext()
            {
                Key = "www",
                StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "WWW")
            }),
            new AccountingService(new WebServiceContext()
            {
                Key = "acctg",
                StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "ACCTG")
            }),
            new BusinessService(new WebServiceContext()
            {
                Key = "biz",
                StaticPath = Path.Combine(Directory.GetCurrentDirectory(), "BIZ")
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
                        break;
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