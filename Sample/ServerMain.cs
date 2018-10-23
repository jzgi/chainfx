using Greatbone;

namespace Samp
{
    public class ServerMain : ServiceUtility
    {
        /// <summary>
        /// The entry point.
        /// </summary>
        public static void Main(string[] args)
        {
            bool deploy = args.Length > 0 && args[0] == nameof(deploy);

            Mount<SampService>(new ServiceConfig("samp")
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