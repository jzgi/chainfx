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
            var pg = new DbConfig
            {
                host = "106.14.45.109",
                port = 5432,
                username = "postgres",
                password = "GangShang721004",
                msg = false
            };

            Obj peers = new Obj{
                new Member("shop-02", "127.0.0.1:7772"),
                new Member("shop-03", "127.0.0.1:7773"),
                new Member("shop-04", "127.0.0.1:7774")
            };

            var www = new WwwService(new WebConfig
            {
                key = "www",
                outer = "127.0.0.1:8080",
                inner = "127.0.0.1:7770",
                peers = peers,
                db = pg
            }
#if !DEBUG
                .Load()
#endif
            );

            var shop = new ShopService(new WebConfig
            {
                key = "shop",
                outer = "127.0.0.1:8081",
                inner = "127.0.0.1:7771",
                peers = peers,
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