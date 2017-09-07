using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Program : Application
    {
        ///
        /// The application entry point.
        ///
        public static void Main(string[] args)
        {
            bool deploy = args.Length > 0;

            Db pg = new Db
            {
                host = "106.14.45.109",
                port = 5432,
                database = "shop",
                username = "postgres",
                password = "721004"
            };

            WeiXinUtility.Setup("weixin.json", "apiclient_cert.p12", deploy);

            // the gospel service
            TryCreate<GospelService>(
                new ServiceContext("gospel")
                {
                    addrs = new[] {"http://localhost:8081"},
                    cipher = 0x4a78be76,
                    cache = true,
                    db = pg
                },
                deploy
            );

            // the health and shopping service
            TryCreate<BuyService>(
                new ServiceContext("buy")
                {
                    addrs = new[] {"http://localhost:8080"},
                    cipher = 0x4a78be76,
                    cache = true,
                    db = pg
                },
                deploy
            );

            StartAll();
        }
    }
}