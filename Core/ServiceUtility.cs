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
    /// <summary>
    /// The global service host that creates, starts and (gracefully) stops service instances.
    /// </summary>
    public class ServiceUtility
    {
        const string CONFIG = "$service.json";

        internal static readonly Lifetime Lifetime = new Lifetime();

        internal static readonly LibuvTransportFactory Libuv = new LibuvTransportFactory(Options.Create(new LibuvTransportOptions()), Lifetime, NullLoggerFactory.Instance);

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
                    JsonParser p = new JsonParser(bytes, bytes.Length);
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

        static readonly CancellationTokenSource Cts = new CancellationTokenSource();

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