using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Program
    {
        ///
        /// The program's entry point.
        ///
        public static void Main(string[] args)
        {
            DbConfig pg = new DbConfig
            {
                host = "106.14.45.109",
                port = 5432,
                username = "postgres",
                password = "GangShang721004",
                queue = false
            };

            List<WebService> services = new List<WebService>(4);

            WebConfig cfg;

            if ((cfg = new WebConfig("www")
            {
                outer = "http://127.0.0.1:8000",
                inner = "http://127.0.0.1:7000",
                refs = new Obj{
                    new Member("shop-01", "http://127.0.0.1:7002"),
                },
                db = pg
            }).LoadJson())
            {
                services.Add(new WwwService(cfg));
            }

            if ((cfg = new WebConfig("shop")
            {
                shard = "01",
                outer = "http://127.0.0.1:8001",
                inner = "http://127.0.0.1:7001",
                refs = new Obj{
                    new Member("shop-01", "http://127.0.0.1:7002"),
                },
                db = pg
            }).LoadJson())
            {
                services.Add(new ShopService(cfg));
            }

            WebService.Run(services);
        }
    }
}