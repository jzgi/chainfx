using Greatbone.Core;

namespace Greatbone.Samp
{
    public class Program : ServiceUtility
    {
        /// <summary>
        /// The application entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            bool deploy = args.Length > 0;

            WeiXinUtility.Setup("weixin.json", "apiclient_cert.p12", deploy);

            // the only www service
            TryCreate<SampService>(
                new ServiceConfig("samp")
                {
                    addrs = new[] {"http://localhost:8080"},
                    cipher = 0x4a78be76,
                    cache = true,
                    db = new Db
                    {
                        host = "106.14.45.109",
                        port = 5432,
                        username = "postgres",
                        password = "721004"
                    },
                },
                deploy
            );

            StartAll();
        }
    }
}