using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SkyChain.Db;
using SkyChain.Web;

namespace SkyChain
{
    /// <summary>
    /// The application scope that holds global states.
    /// </summary>
    public class Application : Chain
    {
        public const string APP_JSON = "app.json";

        public const string CERT_PFX = "cert.pfx";

        internal static readonly WebLifetime Lifetime = new WebLifetime();

        internal static readonly ITransportFactory TransportFactory = new SocketTransportFactory(Options.Create(new SocketTransportOptions()), Lifetime, NullLoggerFactory.Instance);


        // logging level
        static int logging;

        static string crypto;

        static uint[] privatekey;

        static string certpass;

        // layered configurations
        public static readonly JObj chaincfg, webcfg, extcfg;

        static readonly X509Certificate2 cert;

        static readonly ApplicationLogger logger;

        static readonly Map<string, WebService> services = new Map<string, WebService>(4);


        /// <summary>
        /// Load the configuration file and initialize the environments.
        /// </summary>
        static Application()
        {
            // load the config file
            var bytes = File.ReadAllBytes(APP_JSON);
            var parser = new JsonParser(bytes, bytes.Length);
            var cfg = (JObj) parser.Parse();

            // file-based logger
            logging = cfg[nameof(logging)];
            var logfile = DateTime.Now.ToString("yyyyMM") + ".log";
            logger = new ApplicationLogger(logfile)
            {
                Level = logging
            };

            // security settings
            crypto = cfg[nameof(crypto)];
            privatekey = CryptoUtility.HexToKey(crypto);
            certpass = cfg[nameof(certpass)];

            // create cert
            if (certpass != null)
            {
                try
                {
                    cert = new X509Certificate2(File.ReadAllBytes(CERT_PFX), certpass);
                }
                catch (Exception e)
                {
                    WAR(e.Message);
                }
            }

            // init layers
            //

            chaincfg = cfg["chain"];
            if (chaincfg != null)
            {
                InitializeChain(chaincfg);
            }

            webcfg = cfg["web"];
            if (webcfg != null)
            {
                InitializeWeb(webcfg);
            }

            extcfg = cfg["ext"];
        }

        internal static void InitializeWeb(JObj webcfg)
        {
        }


        public static uint[] PrivateKey => privatekey;

        public static ApplicationLogger Logger => logger;

        public static X509Certificate2 Cert => cert;


        public static T MakeService<T>(string name) where T : WebService, new()
        {
            if (webcfg == null)
            {
                throw new ApplicationException("Missing 'web' in config");
            }

            string addr = webcfg[name];
            if (addr == null)
            {
                throw new ApplicationException("Missing web '" + name + "' in config");
            }

            // create service
            var svc = new T
            {
                Name = name,
                Address = addr
            };
            services.Add(name, svc);

            svc.OnMake();
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


        /// <summary>
        /// Runs a number of web services and then block until shutdown.
        /// </summary>
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
    }
}