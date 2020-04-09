using System.Threading;
using System.Threading.Tasks;
using SkyCloud;

namespace SkyCloud.Chain
{
    public class ChainApp : Framework
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

            CreateService<ChainService>("chain");

            await StartAsync();
        }


        public static void DbCaches()
        {
        }
    }
}