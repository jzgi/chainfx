using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Greatbone.Core
{
    public class ServiceUtility
    {
        const string CONFIG = "$service.json";

        internal static readonly Lifetime Lifetime = new Lifetime();

        internal static readonly LibuvTransportFactory LibUv = new LibuvTransportFactory(Options.Create(new LibuvTransportOptions()), Lifetime, NullLoggerFactory.Instance);

        static readonly List<Service> services = new List<Service>(8);

        public static S TryCreate<S>(ServiceConfig cfg, bool load) where S : Service
        {
            // initialize config
            cfg.Parent = null;
            cfg.Level = 0;
            cfg.IsVar = false;
            cfg.Directory = cfg.Name;
            // may load from the configuration file
            if (load)
            {
                string file = cfg.GetFilePath(CONFIG);
                if (File.Exists(file))
                {
                    byte[] bytes = File.ReadAllBytes(file);
                    JsonParse p = new JsonParse(bytes, bytes.Length);
                    JObj jo = (JObj) p.Parse();
                    // this will override values
                    cfg.Read(jo, 0xff);
                }
                else
                {
                    return null;
                }
            }
            // create service instance by reflection
            Type typ = typeof(S);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(ServiceConfig)});
            if (ci == null)
            {
                throw new ServiceException(typ + " missing ServiceContext");
            }
            S inst = (S) ci.Invoke(new object[] {cfg});
            services.Add(inst);
            return inst;
        }

        /// 
        /// Runs a number of web services and block until shutdown.
        /// 
        public static void StartAll()
        {
            using (var cts = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    cts.Cancel();
                    // wait for the Main thread to exit gracefully.
                    eventArgs.Cancel = true;
                };

                // start services
                foreach (Service svc in services)
                {
                    svc.StartAsync();
                }

                Console.WriteLine("ctrl_c to shut down");

                cts.Token.Register(state =>
                {
                    ((IApplicationLifetime) state).StopApplication();
                    foreach (Service svc in services) // dispose services
                    {
                        svc.OnStop(); // call custom destruction
                        svc.Dispose();
                    }
                }, Lifetime);
                Lifetime.ApplicationStopping.WaitHandle.WaitOne();
            }
        }
    }

    internal class Lifetime : IApplicationLifetime
    {
        readonly CancellationTokenSource started = new CancellationTokenSource();

        readonly CancellationTokenSource stopping = new CancellationTokenSource();

        readonly CancellationTokenSource stopped = new CancellationTokenSource();

        /// 
        /// Triggered when the application host has fully started and is about to wait for a graceful shutdown.
        /// 
        public CancellationToken ApplicationStarted => started.Token;

        /// 
        /// Triggered when the application host is performing a graceful shutdown. Request may still be in flight. Shutdown will block until this event completes.
        /// 
        public CancellationToken ApplicationStopping => stopping.Token;

        /// 
        /// Triggered when the application host is performing a graceful shutdown. All requests should be complete at this point. Shutdown will block until this event completes.
        /// 
        public CancellationToken ApplicationStopped => stopped.Token;

        /// 
        /// Signals the ApplicationStopping event and blocks until it completes.
        /// 
        public void StopApplication()
        {
            // Lock on CTS to synchronize multiple calls to StopApplication. This guarantees that the first call 
            // to StopApplication and its callbacks run to completion before subsequent calls to StopApplication, 
            // which will no-op since the first call already requested cancellation, get a chance to execute.
            lock (stopping)
            {
                try
                {
                    stopping.Cancel(false);
                }
                catch (Exception)
                {
                }
            }
        }

        ///
        /// Signals the ApplicationStarted event and blocks until it completes.
        ///
        public void NotifyStarted()
        {
            try
            {
                started.Cancel(false);
            }
            catch (Exception)
            {
            }
        }

        /// 
        /// Signals the ApplicationStopped event and blocks until it completes.
        /// 
        public void NotifyStopped()
        {
            try
            {
                stopped.Cancel(false);
            }
            catch (Exception)
            {
            }
        }
    }
}