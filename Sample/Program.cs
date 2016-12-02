using Greatbone.Core;

namespace Greatbone.Sample
{
    public partial class Program
    {
        ///
        /// The program's entry point.
        ///
        public static void Main(string[] args)
        {
            // var pg = new DbConfig
            // {
            //     host = "34.65.112.67",
            //     port = 5432,
            //     username = "postgres",
            //     password = "1111111",
            //     msg = false
            // };

            var www = new WwwService(new WebConfig
            {
                name = "www",
                outer = "http://127.0.0.1:8080",
                inner = "http://127.0.0.1:7770",
                refs = new Obj{
                    new Member("shop-01", "http://127.0.0.1:7772"),
                },
                db = pg
            }
#if !DEBUG
                .Load()
#endif
            );

            var shop = new ShopService(new WebConfig
            {
                name = "shop",
                partition = "01",
                outer = "http://127.0.0.1:8081",
                inner = "http://127.0.0.1:7771",
                refs = new Obj{
                    new Member("www", "http://127.0.0.1:7770"),
                },
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