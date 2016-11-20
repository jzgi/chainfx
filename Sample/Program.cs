using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Program
    {
        ///
        /// The entry point of the application.
        ///
        public static void Main(string[] args)
        {
            var pg = new DbConfig
            {
                host = "106.14.45.109",
                port = 5432,
                username = "postgres",
                password = "GangShang721004",
                msg = false
            };

            string[] addrs =
            {
                "127.0.0.1:7070", "127.0.0.1:7071"
            };

            var www = new WwwService(new WebConfig
            {
                key = "www",
                @extern = "127.0.0.1:8080",
                intern = "127.0.0.1:7070",
                net = addrs,
                db = pg
            }
#if !DEBUG
                .Load()
#endif
            );

            var shop = new ShopService(new WebConfig
            {
                key = "shop",
                @extern = "127.0.0.1:8081",
                intern = "127.0.0.1:7071",
                net = addrs,
                db = pg
            }
#if !DEBUG
                .Load()
#endif
            );

            WebService.Run(www, shop);
        }
    }
}