using System.Threading;
using System.Threading.Tasks;
using CloudUn;

namespace CloudUn.Net
{
    public class NetApp : Framework
    {
        static Timer timer;

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        public static async Task Main(string[] args)
        {
            DbCaches();


            // var cert = BuildSelfSignedCertificate("144000.tv", "47.100.96.253", "144000.tv", "721004");
            // var bytes = cert.Export(X509ContentType.Pfx);
            // File.WriteAllBytes("samp/$cert.pfx", bytes);

            // CreateService<ChainService>("chain");

            CreateService<NetService>("care");

            await StartAsync();
        }


        public static void DbCaches()
        {
        }
    }
}