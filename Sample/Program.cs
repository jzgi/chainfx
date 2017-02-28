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
                new JMbr("op", "http://localhost:8080#00000000"),
                new JMbr("comm", "http://localhost:8081"),
            };

#if DEBUG
            ServiceUtility.Create<OpService>("op", new JObj{
                new JMbr("address", "http://localhost:8080"),
                new JMbr("auth", auth),
                new JMbr("db", pg),
                new JMbr("cluster", cluster),
            });
#else
            ServiceUtility.Create<OpService>("op");
#endif

#if DEBUG
            ServiceUtility.Create<OpService>("comm", new JObj{
                new JMbr("address", "http://localhost:8081"),
                new JMbr("auth", auth),
                new JMbr("db", pg),
                new JMbr("cluster", cluster),
            });
#else
            ServiceUtility.Create<OpService>("comm");
#endif

            ServiceUtility.StartAll();
        }
    }
}