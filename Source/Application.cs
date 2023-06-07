using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ChainFx.Nodal;
using ChainFx.Web;
using Microsoft.AspNetCore.Connections;
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
        public const string
            APP_JSON = "app.json",
            CERT_PFX = "cert.pfx";


        static readonly string name;

        static readonly int logging;

        static readonly string secret;

        // config
        internal static readonly JObj app;

        internal static readonly JObj prog;


        // X509 certificate
        static readonly X509Certificate2 cert;

        // the global logger
        internal static readonly FileLogger logger;


        internal static readonly WebLifetime Lifetime = new WebLifetime();

        internal static readonly IConnectionListenerFactory TransportFactory;


        // registered web services
        static readonly Map<string, WebService> services = new(8);


        // registered web connectors
        static readonly Map<string, WebConnector> connectors = new(32);


        /// <summary>
        /// The process of configuration and initialization.
        /// </summary>
        static Application()
        {
            // load the app config
            //

            var bytes = File.ReadAllBytes(APP_JSON);
            var parser = new JsonParser(bytes, bytes.Length);
            app = (JObj)parser.Parse();

            name = app[nameof(name)];

            // logging and logger
            //

            logging = app[nameof(logging)];

            var file = DateTime.Now.ToString("yyyyMM") + ".log";
            logger = new FileLogger(file)
            {
                Level = logging
            };

            TransportFactory = new SocketTransportFactory(
                Options.Create(new SocketTransportOptions()), NullLoggerFactory.Instance
            );

            // security
            //
            secret = app[nameof(secret)];

            // X509 certificate
            //

            string certpasswd = app[nameof(certpasswd)];
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

            // db config
            //

            JObj db = app[nameof(db)];
            if (db != null)
            {
                InitNodality(db);
            }

            prog = app[nameof(prog)];
        }

        public static JObj App => app;

        public static JObj Prog => prog;

        public static string Name => name;

        public static int Logging => logging;

        public static FileLogger Logger => logger;

        public static X509Certificate2 Certificate => cert;

        public static string Secret => secret;


        // ReSharper disable once ParameterHidesMember
        public static S CreateService<S>(string name, string folder = null) where S : WebService, new()
        {
            if (app == null)
            {
                throw new ApplicationException("missing app.json");
            }

            // web config
            //

            var prop = "service-" + name;
            var servicecfg = (JObj)app[prop];

            if (servicecfg == null)
            {
                throw new ApplicationException("missing '" + prop + "' in app.json");
            }

            // create service (properties in order)
            var svc = new S
            {
                Name = name,
                Folder = folder ?? name
            };
            svc.Init(prop, servicecfg);

            services.Add(name, svc);

            // invoke on creatte
            svc.OnCreate();

            return svc;
        }


        /// <summary>
        /// Runs a number of web services and then block until shutdown.
        /// </summary>
        public static async Task StartAsync(bool waiton = true)
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
            //

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                Canceller.Cancel(false);
                exitevt.Set(); // release the Main thread
            };
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                Canceller.Cancel(false);
                exitevt.Set(); // release the Main thread
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
            Console.WriteLine("CTRL + C to shut down");

            Lifetime.NotifyStarted();

            if (waiton)
            {
                // wait on the reset event
                exitevt.Wait(Canceller.Token);

                await StopAsync();
            }
        }

        public static async Task StopAsync()
        {
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

        static readonly CancellationTokenSource Canceller = new();
    }
}