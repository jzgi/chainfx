using Greatbone;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace Core
{
    /// <summary>
    /// For easy sharing code between works.
    /// </summary>
    public interface IOrgVar
    {
    }

    public class Program : ServiceUtility
    {
        /// <summary>
        /// The application entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            bool deploy = args.Length > 0;

            WeiXinUtility.Setup("weixin.json", deploy, "apiclient_cert.p12");

            TryCreate<CoreService>(
                new ServiceConfig("core")
                {
                    addrs = new[] {"http://localhost:8080"},
                    cipher = 0x4a78be76,
                    db = new Db
                    {
                        host = "144000.tv",
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