using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Program : Application
    {
        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        ///
        /// The application entry point.
        ///
        public static void Main(string[] args)
        {
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

            Map cluster = new Map
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
                !IsDebug()
            );

            TryCreate<ChatService>(
                new ServiceContext("chat")
                {
                    addrs = new[] { "http://localhost:8081" },
                    auth = auth,
                    db = pg,
                    cluster = cluster
                },
                !IsDebug()
            );

            StartAll();
        }
    }
}