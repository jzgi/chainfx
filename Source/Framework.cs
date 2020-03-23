using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CloudUn.Db;
using CloudUn.Net;
using CloudUn.Web;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace CloudUn
{
    /// <summary>
    /// The application scope that holds global states.
    /// </summary>
    public class Framework
    {
        public const string APP_JSON = "app.json";

        public const string CERT_PFX = "cert.pfx";

        internal static readonly WebLifetime Lifetime = new WebLifetime();

        internal static readonly ITransportFactory TransportFactory = new SocketTransportFactory(Options.Create(new SocketTransportOptions()), Lifetime, NullLoggerFactory.Instance);

        //
        // configuration processing
        //

        // logging level
        internal static readonly int logging = 3;

        internal static readonly int cipher;

        internal static readonly string sign;

        internal static readonly string certpasswd;

        public static readonly JObj WEB, DB, NET, EXT; // various config parts


        static readonly Map<string, WebService> services = new Map<string, WebService>(4);

        static readonly Map<string, NetClient> peers = new Map<string, NetClient>(32);

        static readonly DbSource dbsource;

        internal static readonly FrameworkLogger Logger;


        static List<NetClient> polls = null;

        // the thread schedules and drives periodic jobs, such as event polling 
        static Thread scheduler;

        static Framework()
        {
            // load configuration
            //
            byte[] bytes = File.ReadAllBytes(APP_JSON);
            var parser = new JsonParser(bytes, bytes.Length);
            var cfg = (JObj) parser.Parse();

            logging = cfg[nameof(logging)];
            cipher = cfg[nameof(cipher)];
            sign = cipher.ToString();
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

            // setup chain net peer references
            NET = cfg["NET"];
            if (NET != null)
            {
                for (var i = 0; i < NET.Count; i++)
                {
                    var e = NET.At(i);
                    peers.Add(new NetClient(e.Key, e.value)
                    {
                        Clustered = true
                    });
                }
            }

            // setup the only db source
            DB = cfg["DB"];
            if (DB != null)
            {
                dbsource = new DbSource(DB);
            }

            // create and start the scheduler thead
            if (polls != null)
            {
                // to repeatedly check and initiate event polling activities.
                scheduler = new Thread(() =>
                {
                    while (true)
                    {
                        // interval
                        Thread.Sleep(1000);

                        // a schedule cycle
                        int tick = Environment.TickCount;
                        for (int i = 0; i < polls.Count; i++)
                        {
                            var cli = polls[i];
                            cli.TryPollAsync(tick);
                        }
                    }
                });
                scheduler.Start();
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

            using (RSA rsa = RSA.Create(2048))
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

        //
        // db-object cache
        //

        public static DbSource DbSource() => dbsource;

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            if (dbsource == null)
            {
                throw new FrameworkException("missing DB in " + APP_JSON);
            }
            return dbsource.NewDbContext(level);
        }

        static Cell[] cells;

        static int size;

        static ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        public static void Cache(object value, byte flag = 0)
        {
            if (cells == null)
            {
                cells = new Cell[32];
            }

            cells[size++] = new Cell(value, flag);
        }

        public static void Cache<V>(Func<DbContext, V> fetch, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new Cell[8];
            }

            cells[size++] = new Cell(typeof(V), fetch, maxage, flag);
        }

        public static void Cache<V>(Func<DbContext, Task<V>> fetchAsync, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new Cell[8];
            }

            cells[size++] = new Cell(typeof(V), fetchAsync, maxage, flag);
        }

        /// <summary>
        /// Search for typed object in this scope and the scopes of ancestors; 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>the result object or null</returns>
        public static T Obtain<T>(byte flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var c = cells[i];
                    if (c.Flag == 0 || (c.Flag & flag) > 0)
                    {
                        if (!c.IsAsync && typeof(T).IsAssignableFrom(c.Typ))
                        {
                            return c.GetValue() as T;
                        }
                    }
                }
            }

            return null;
        }

        public static async Task<T> ObtainAsync<T>(byte flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var cell = cells[i];
                    if (cell.Flag == 0 || (cell.Flag & flag) > 0)
                    {
                        if (cell.IsAsync && typeof(T).IsAssignableFrom(cell.Typ))
                        {
                            return await cell.GetValueAsync() as T;
                        }
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// A object holder in registry.
        /// </summary>
        class Cell
        {
            readonly Type typ;

            readonly Func<DbContext, object> fetch;

            readonly Func<DbContext, Task<object>> fetchAsync;

            readonly int maxage; //in seconds

            // tick count,   
            int expiry;

            object value;

            readonly byte flag;

            internal Cell(object value, byte flag)
            {
                this.typ = value.GetType();
                this.value = value;
                this.flag = flag;
            }

            internal Cell(Type typ, Func<DbContext, object> fetch, int maxage, byte flag)
            {
                this.typ = typ;
                this.flag = flag;
                if (fetch is Func<DbContext, Task<object>> fetch2)
                {
                    this.fetchAsync = fetch2;
                }
                else
                {
                    this.fetch = fetch;
                }

                this.maxage = maxage;
            }

            public Type Typ => typ;

            public byte Flag => flag;

            public bool IsAsync => fetchAsync != null;

            public object GetValue()
            {
                if (fetch == null) // simple object
                {
                    return value;
                }

                @lock.EnterUpgradeableReadLock();
                try
                {
                    if (Environment.TickCount >= expiry)
                    {
                        @lock.EnterWriteLock();
                        try
                        {
                            using var dc = NewDbContext();
                            value = fetch(dc);
                            expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
                        }
                        finally
                        {
                            @lock.ExitWriteLock();
                        }
                    }
                    return value;
                }
                finally
                {
                    @lock.ExitUpgradeableReadLock();
                }
            }

            public async Task<object> GetValueAsync()
            {
                if (fetchAsync == null) // simple object
                {
                    return value;
                }

                int lexpiry = this.expiry;
                int ticks = Environment.TickCount;
                if (ticks >= lexpiry)
                {
                    using var dc = NewDbContext();
                    value = await fetchAsync(dc);
                    expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
                }

                return value;
            }
        }
    }
}