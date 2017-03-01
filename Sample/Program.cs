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
            JObj auth = new JObj{
                new JMbr("mask", 0x4a78be76),
                new JMbr("pose", 0x1f0335e2),
                new JMbr("maxage", 360000),
            };

            JObj pg = new JObj{
                new JMbr("host", "106.14.45.109"),
                new JMbr("port", 5432),
                new JMbr("username", "postgres"),
                new JMbr("password", "721004"),
            };

            JObj cluster = new JObj{
                new JMbr("op", "http://localhost:8080"),
                new JMbr("comm", "http://localhost:8081"),
            };

            JObj wexin = new JObj{
                new JMbr("appid", "wxf2820fd58bf8745f"),
                new JMbr("appsecret", "85912d26f108b6a3d9fd2fa4aa8f0b83"),
            };

            Create<OpService>("op",
#if DEBUG
            new JObj{
                new JMbr("addresses", new JArr("http://localhost:8080")),
                new JMbr("auth", auth),
                new JMbr("db", pg),
                new JMbr("cluster", cluster)
            }
#endif
            );

            Create<CommService>("comm",
#if DEBUG
            new JObj{
                new JMbr("addresses", new JArr("http://localhost:8081")),
                new JMbr("auth", auth),
                new JMbr("db", pg),
                new JMbr("cluster", cluster),
            }
#endif
            );

            StartAll();
        }
    }
}