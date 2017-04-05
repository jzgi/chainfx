using System.Collections.Generic;
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

            Auth auth = new Auth
            {
                mask = 0x4a78be76,
                pose = 0x1f0335e2,
                maxage = 360000
            };

            Db pg = new Db
            {
                host = "106.14.45.109",
                port = 5432,
                username = "postgres",
                password = "721004"
            };

            Dictionary<string, string> cluster = new Dictionary<string, string>
            {
                ["shop"] = "http://localhost:8080",
                ["chat"] = "http://localhost:8081"
            };

            TryCreate<ShopService>(
                new ServiceContext("shop")
                {
                    addrs = new[] { "http://localhost:8080" },
                    auth = auth,
                    db = pg,
                    cluster = cluster
                },
                deploy
            );

            TryCreate<ChatService>(
                new ServiceContext("chat")
                {
                    addrs = new[] { "http://localhost:8081" },
                    auth = auth,
                    db = pg,
                    cluster = cluster
                },
                deploy
            );

            StartAll();
        }
    }
}