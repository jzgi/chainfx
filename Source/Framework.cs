using System;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SkyCloud.Chain;
using SkyCloud.Db;
using SkyCloud.Web;

namespace SkyCloud
{
    /// <summary>
    /// The application scope that holds global states.
    /// </summary>
    public class Framework : Chain.Chain
    {
        public const string APP_JSON = "app.json";

        public const string CERT_PFX = "cert.pfx";

        internal static readonly WebLifetime Lifetime = new WebLifetime();

        internal static readonly ITransportFactory TransportFactory = new SocketTransportFactory(Options.Create(new SocketTransportOptions()), Lifetime, NullLoggerFactory.Instance);

        //
        // configuration processing
        //

        // logging level
        internal static readonly int logging;

        internal static readonly string crypto;

        internal static readonly uint[] privatekey;

        internal static readonly uint[] publickey = {0x4f738f17, 0x55ce9124, 0x012f4a98, 0x6d85e342};

        internal static readonly string certpasswd;

        public static readonly JObj WEB, DB, EXT; // various config parts


        static readonly Map<string, WebService> services = new Map<string, WebService>(4);

        internal static readonly FrameworkLogger Logger;


        static Framework()
        {
            // load configuration
            //
            byte[] bytes = File.ReadAllBytes(APP_JSON);
            var parser = new JsonParser(bytes, bytes.Length);
            var cfg = (JObj) parser.Parse();

            logging = cfg[nameof(logging)];
            crypto = cfg[nameof(crypto)];
            privatekey = CryptionUtility.HexToKey(crypto);
            certpasswd = cfg[nameof(certpasswd)];

            // setup logger first
            //
            string file = DateTime.Now.ToString("yyyyMM") + ".log";
            Logger = new FrameworkLogger(file)
            {
                Level = logging
            };
            if (!File.Exists(APP_JSON))
            {
                Logger.Log(LogLevel.Error, APP_JSON + " file not found");
                return;
            }

            WEB = cfg["WEB"];
            EXT = cfg["EXT"];
            DB = cfg["DB"];
            if (DB != null) // setup the only db source
            {
                dbsource = new DbSource(DB);
            }
        }

        public static T CreateService<T>(string name) where T : WebService, new()
        {
            if (WEB == null)
            {
                throw new FrameworkException("Missing 'WEB' in " + APP_JSON);
            }

            string addr = WEB[name];
            if (addr == null)
            {
                throw new FrameworkException("missing WEB '" + name + "' in " + APP_JSON);
            }

            // create service
            var svc = new T
            {
                Name = name,
                Address = addr
            };
            services.Add(name, svc);

            svc.OnCreate();
            return svc;
        }


        //
        // logging methods
        //

        public static void TRC(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Trace, 0, msg, ex, null);
            }
        }

        public static void DBG(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Debug, 0, msg, ex, null);
            }
        }

        public static void INF(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Information, 0, msg, ex, null);
            }
        }

        public static void WAR(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Warning, 0, msg, ex, null);
            }
        }

        public static void ERR(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Error, 0, msg, ex, null);
            }
        }


        static readonly CancellationTokenSource Canceller = new CancellationTokenSource();


        /// 
        /// Runs a number of web services and block until shutdown.
        /// 
        public static async Task StartAsync()
        {
            var exitevt = new ManualResetEventSlim(false);

            // start all services
            //
            for (int i = 0; i < services.Count; i++)
            {
                var svc = services.ValueAt(i);
                await svc.StartAsync(Canceller.Token);
            }

            // handle SIGTERM and CTRL_C 
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                Canceller.Cancel(false);
                exitevt.Set(); // release the Main thread
            };
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Canceller.Cancel(false);
                exitevt.Set(); // release the Main thread
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
            Console.WriteLine("CTRL + C to shut down");

            Lifetime.NotifyStarted();

            // wait on the reset event
            exitevt.Wait(Canceller.Token);

            Lifetime.StopApplication();

            for (int i = 0; i < services.Count; i++)
            {
                var svc = services.ValueAt(i);
                await svc.StopAsync(Canceller.Token);
            }

            Lifetime.NotifyStopped();
        }

        public static X509Certificate2 BuildSelfSignedCertificate(string dns, string ipaddr, string issuer, string password)
        {
            var sanb = new SubjectAlternativeNameBuilder();
            sanb.AddIpAddress(IPAddress.Parse(ipaddr));
            sanb.AddDnsName(dns);

            var subject = new X500DistinguishedName($"CN={issuer}");

            using (var rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

                request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection {new Oid("1.3.6.1.5.5.7.3.1")}, false));

                request.CertificateExtensions.Add(sanb.Build());

                var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
                certificate.FriendlyName = issuer;

                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            }
        }
    }
}