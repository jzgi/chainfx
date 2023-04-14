using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ChainFx.Nodal;
using ChainFx.Web;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ChainFx
{
    /// <summary>
    /// The web application scope that holds global states.
    /// </summary>
    public abstract class Application : Nodality
    {
        public const string APP_JSON = "app.json";

        public const string CERT_PFX = "cert.pfx";

        static readonly string secret;

        // config
        internal static readonly JObj app;

        internal static readonly JObj prog;


        // X509 certificate
        static readonly X509Certificate2 cert;

        // the global logger
        internal static readonly FileLogger logger;


        internal static readonly WebLifetime Lifetime = new WebLifetime();

        internal static readonly ITransportFactory TransportFactory;


        // registered services
        static readonly Map<string, WebService> services = new Map<string, WebService>(4);


        /// <summary>
        /// Load the configuration and initialize.
        /// </summary>
        static Application()
        {
            // load app config
            var bytes = File.ReadAllBytes(APP_JSON);
            var parser = new JsonParser(bytes, bytes.Length);
            app = (JObj)parser.Parse();

            // file-based logger
            int logging = app[nameof(logging)];
            var logfile = DateTime.Now.ToString("yyyyMM") + ".log";
            logger = new FileLogger(logfile)
            {
                Level = logging
            };

            TransportFactory = new SocketTransportFactory(Options.Create(new SocketTransportOptions()), Lifetime,
                NullLoggerFactory.Instance);

            // security
            //
            secret = app[nameof(secret)];

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
                    War(e.Message);
                }
            }

            // fabric and nodality cfg
            JObj nodal = app[nameof(nodal)];
            if (nodal != null)
            {
                InitializeNodal(nodal);
            }

            prog = app[nameof(prog)];
        }

        public static JObj App => app;

        public static JObj Prog => prog;


        public static string Secret => secret;

        public static FileLogger Logger => logger;

        public static X509Certificate2 Certificate => cert;


        public static S CreateService<S>(string name, string folder = null) where S : WebService, new()
        {
            if (app == null)
            {
                throw new ApplicationException("missing app.json");
            }

            // web config
            //

            var prop = "web-" + name;
            var webcfg = (JObj)app[prop];

            if (webcfg == null)
            {
                throw new ApplicationException("missing '" + prop + "' in app.json");
            }

            // create service (properties in order)
            var svc = new S
            {
                Name = name,
                Folder = folder ?? name
            };
            svc.Initialize(prop, webcfg);

            services.Add(name, svc);

            // invoke on creatte
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
            try
            {
                for (int i = 0; i < services.Count; i++)
                {
                    var svc = services.ValueAt(i);

                    await svc.StartAsync(Canceller.Token);
                }
            }
            catch (Exception e)
            {
                Err(e.Message);
                throw;
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

        public static void Trc(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Trace, 0, msg, ex, null);
            }
        }

        public static void Dbg(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Debug, 0, msg, ex, null);
            }
        }

        public static void Inf(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Information, 0, msg, ex, null);
            }
        }

        public static void War(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Warning, 0, msg, ex, null);
            }
        }

        public static void Err(string msg, Exception ex = null)
        {
            if (msg != null)
            {
                Logger.Log(LogLevel.Error, 0, msg, ex, null);
            }
        }

        static readonly CancellationTokenSource Canceller = new CancellationTokenSource();
    }
}