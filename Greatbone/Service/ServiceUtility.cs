using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Greatbone.Service
{
    /// <summary>
    /// The global service host that creates, starts and (gracefully) stops service instances.
    /// </summary>
    public class ServiceUtility
    {
        public const string SERVICE_JSON = "$service.json";

        public const string CERT_PFX = "$cert.pfx";

        internal static readonly Lifetime Lifetime = new Lifetime();

        internal static readonly ITransportFactory TransportFactory = new SocketTransportFactory(Options.Create(new SocketTransportOptions()), Lifetime, NullLoggerFactory.Instance);

        static readonly List<Service> services = new List<Service>(8);

        /// <summary>
        /// To mount a service with the underlying file folder and a designated HTTP endpoint
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="loadCfg"></param>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public static S Mount<S>(ServiceConfig cfg, bool loadCfg) where S : Service
        {
            // initialize config
            cfg.Parent = null;
            cfg.Level = 0;
            cfg.IsVar = false;
            cfg.Directory = cfg.Name;
            cfg.Pathing = "/";

            if (!Directory.Exists(cfg.Directory))
            {
                return null;
            }

            // may load from the configuration file
            if (loadCfg)
            {
                string webfile = cfg.GetFilePath(SERVICE_JSON);
                if (!File.Exists(webfile)) return null;

                byte[] bytes = File.ReadAllBytes(webfile);
                JsonParser p = new JsonParser(bytes, bytes.Length);
                JObj jo = (JObj) p.Parse();
                // this will override values
                cfg.Read(jo, 0xff);
            }

            // create service instance by reflection
            Type typ = typeof(S);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(ServiceConfig)});
            if (ci == null)
            {
                throw new ServiceException(typ + " missing ServiceConfig");
            }

            S inst = (S) ci.Invoke(new object[] {cfg});
            services.Add(inst);
            return inst;
        }

        static readonly CancellationTokenSource Cts = new CancellationTokenSource();

        public static Service GetService(string name = null)
        {
            if (name == null)
            {
                return services[0];
            }
            else
            {
                for (int i = 0; i < services.Count; i++)
                {
                    var svc = services[i];
                    if (svc.Name == name) return svc;
                }
            }
            return null;
        }

        /// 
        /// Runs a number of web services and block until shutdown.
        /// 
        public static void StartAll()
        {
            var exit = new ManualResetEventSlim(false);

            // start service instances
            foreach (Service svc in services)
            {
                svc.StartAsync(Cts.Token).GetAwaiter().GetResult();
            }

            // handle SIGTERM and CTRL_C 
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                Cts.Cancel(false);
                exit.Set(); // release the Main thread
            };
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Cts.Cancel(false);
                exit.Set(); // release the Main thread
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
            Console.WriteLine("CTRL + C to shut down");

            Lifetime.NotifyStarted();

            // wait on the reset event
            exit.Wait(Cts.Token);

            Lifetime.StopApplication();

            foreach (Service svc in services)
            {
                svc.StopAsync(Cts.Token).GetAwaiter().GetResult();
            }

            Lifetime.NotifyStopped();
        }

        public static X509Certificate2 BuildSelfSignedCertificate(string dns, string ipaddr, string issuer, string password)
        {
            SubjectAlternativeNameBuilder sanb = new SubjectAlternativeNameBuilder();
            sanb.AddIpAddress(IPAddress.Parse(ipaddr));
            sanb.AddDnsName(dns);

            X500DistinguishedName subject = new X500DistinguishedName($"CN={issuer}");

            using (RSA rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(new OidCollection {new Oid("1.3.6.1.5.5.7.3.1")}, false));

                request.CertificateExtensions.Add(sanb.Build());

                var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
                certificate.FriendlyName = issuer;

                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            }
        }
    }

    internal class Lifetime : IApplicationLifetime
    {
        readonly CancellationTokenSource started = new CancellationTokenSource();

        readonly CancellationTokenSource stopping = new CancellationTokenSource();

        readonly CancellationTokenSource stopped = new CancellationTokenSource();

        /// triggered when the service host has fully started and is about to wait for a graceful shutdown.
        /// 
        public CancellationToken ApplicationStarted => started.Token;

        /// triggered when the service host is performing a graceful shutdown. Request may still be in flight. Shutdown will block until this event completes.
        /// 
        public CancellationToken ApplicationStopping => stopping.Token;

        /// Triggered when the service host is performing a graceful shutdown. All requests should be complete at this point. Shutdown will block until this event completes.
        /// 
        public CancellationToken ApplicationStopped => stopped.Token;

        /// signals the ApplicationStopping event and blocks until it completes.
        /// 
        public void StopApplication()
        {
            try
            {
                if (stopping.IsCancellationRequested) // already cancelled
                {
                    return;
                }
                stopping.Cancel(false);
            }
            catch (Exception)
            {
            }
        }

        /// Signals the ApplicationStarted event and blocks until it completes.
        ///
        public void NotifyStarted()
        {
            try
            {
                if (started.IsCancellationRequested) // already cancelled
                {
                    return;
                }
                started.Cancel(false);
            }
            catch (Exception)
            {
            }
        }

        /// Signals the ApplicationStopped event and blocks until it completes.
        /// 
        public void NotifyStopped()
        {
            try
            {
                if (stopped.IsCancellationRequested) // already cancelled
                {
                    return;
                }
                stopped.Cancel(false);
            }
            catch (Exception)
            {
            }
        }
    }
}