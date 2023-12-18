using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ChainFX.Nodal;
using ChainFX.Web;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ChainFX
{
    /// <summary>
    /// The web application scope that holds global states.
    /// </summary>
    public abstract class Application : Nodality
    {
        public const string
            APPLICATION_JSON = "application.json",
            CERTIFICATE_PFX = "certificate.pfx";


        static readonly string name;

        static readonly int logging;

        static readonly string secret;

        // config
        internal static readonly JObj config;

        internal static readonly JObj custom;


        // X509 certificate
        static readonly X509Certificate2 certificate;

        // the global logger
        internal static readonly FileLogger logger;


        internal static readonly WebLifetime Lifetime = new WebLifetime();

        internal static readonly IConnectionListenerFactory TransportFactory;


        // registered web services
        static readonly Map<string, WebService> services = new(8);


        // registered web connectors
        static readonly Map<string, WebConnect> connectors = new(32);


        /// <summary>
        /// The process of configuration and initialization.
        /// </summary>
        static Application()
        {
            // load the app config
            //

            var bytes = File.ReadAllBytes(APPLICATION_JSON);
            var parser = new JsonParser(bytes, bytes.Length);
            config = (JObj)parser.Parse();
            name = config[nameof(name)];

            // logging and logger
            //

            logging = config[nameof(logging)];

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
            secret = config[nameof(secret)];

            // X509 certificate
            //

            string certpasswd = config[nameof(certpasswd)];
            if (certpasswd != null)
            {
                try
                {
                    certificate = new X509Certificate2(File.ReadAllBytes(CERTIFICATE_PFX), certpasswd);
                }
                catch (Exception e)
                {
                    War(e.Message);
                }
            }

            // db config
            //

            JObj db = config[nameof(db)];
            if (db != null)
            {
                InitNodality(db);
            }

            custom = config[nameof(custom)];
        }

        /// <summary>
        /// The configuration for the application (application.json).
        /// </summary>
        public static JObj Config => config;

        /// <summary>
        /// The custom section within the application configuration.
        /// </summary>
        public static JObj CustomConfig => custom;

        public static string Name => name;

        public static int Logging => logging;

        public static FileLogger Logger => logger;

        public static X509Certificate2 Certificate => certificate;

        public static string Secret => secret;


        // ReSharper disable once ParameterHidesMember
        public static S CreateService<S>(string name, string folder = null) where S : WebService, new()
        {
            if (config == null)
            {
                throw new ApplicationException("missing " + APPLICATION_JSON);
            }

            // web config
            //

            var prop = "service-" + name;
            var serviceConfig = (JObj)config[prop];

            if (serviceConfig == null)
            {
                throw new ApplicationException("missing '" + prop + "' in " + APPLICATION_JSON);
            }

            // create service (properties in order)
            var svc = new S
            {
                Name = name,
                Folder = folder
            };
            svc.Init(prop, serviceConfig);

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