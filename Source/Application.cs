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
using SkyChain.Nodal;
using SkyChain.Web;

namespace SkyChain
{
    /// <summary>
    /// The web application scope that holds global states.
    /// </summary>
    public abstract class Application : Home
    {
        public const string APP_JSON = "app.json";

        public const string CERT_PFX = "cert.pfx";

        internal static readonly WebLifetime Lifetime = new WebLifetime();

        internal static readonly ITransportFactory TransportFactory = new SocketTransportFactory(Options.Create(new SocketTransportOptions()), Lifetime, NullLoggerFactory.Instance);


        static readonly uint[] cryptokey;

        // layered configurations
        public static readonly JObj app, ext;

        static readonly X509Certificate2 cert;

        static readonly ApplicationLogger logger;


        static readonly Map<string, WebService> services = new Map<string, WebService>(4);


        /// <summary>
        /// Load the configuration and initialize.
        /// </summary>
        static Application()
        {
            // load app config
            var bytes = File.ReadAllBytes(APP_JSON);
            var parser = new JsonParser(bytes, bytes.Length);
            app = (JObj) parser.Parse();

            // file-based logger
            int logging = app[nameof(logging)];
            var logfile = DateTime.Now.ToString("yyyyMM") + ".log";
            logger = new ApplicationLogger(logfile)
            {
                Level = logging
            };

            // security
            //
            string crypto = app[nameof(crypto)];
            cryptokey = CryptoUtility.HexToKey(crypto);

            // string fedkey = app[nameof(fedkey)]; // federal key
            // _fedkey = CryptoUtility.HexToKey(fedkey);

            string certpasswd = app[nameof(certpasswd)]; // X509 certificate
            if (certpasswd != null)
            {
                try
                {
                    cert = new X509Certificate2(File.ReadAllBytes(CERT_PFX), certpasswd);
                }
                catch (Exception e)
                {
                    WAR(e.Message);
                }
            }

            // store
            JObj home = app[nameof(home)];
            if (home != null)
            {
                InitializeHome(home);
            }

            ext = app[nameof(ext)];
        }

        public static JObj App => app;

        public static JObj Ext => ext;

        public static uint[] CryptoKey => cryptokey;

        public static ApplicationLogger Logger => logger;

        public static X509Certificate2 Certificate => cert;


        public static S CreateService<S>(string name) where S : WebService, new()
        {
            if (app == null)
            {
                throw new ApplicationException("missing app.json");
            }

            // web config
            //

            var prop = "web-" + name;
            var webcfg = (JObj) app[prop];

            if (webcfg == null)
            {
                throw new ApplicationException("missing '" + prop + "' in app.json");
            }

            // address (required)
            string address = webcfg[nameof(address)];
            if (address == null)
            {
                throw new ApplicationException("missing 'address' in app.json");
            }

            // optional

            bool cache = webcfg[nameof(cache)];

            string forward = webcfg[nameof(forward)];

            short poll = webcfg[nameof(poll)];

            // create service (properties in order)
            var svc = new S
            {
                Name = name,
                Address = address,
                Cache = cache,
                Forward = forward,
                Poll = poll
            };
            services.Add(name, svc);

            svc.OnCreate();
            return svc;
        }


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
    }
}