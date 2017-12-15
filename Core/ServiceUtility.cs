using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace Greatbone.Core
{
    public class ServiceUtility
    {
        const string CONFIG = "$service.json";

        internal static readonly Lifetime Lifetime = new Lifetime();

        static readonly List<Service> services = new List<Service>(8);

        public static List<Service> Services => services;

        public static S TryCreate<S>(ServiceContext sc, bool load) where S : Service
        {
            // initialize context
            sc.ServiceCtx = sc;
            sc.Parent = null;
            sc.Level = 0;
            sc.Directory = sc.Name;
            if (load) // need to load from configuration file
            {
                string file = sc.GetFilePath(CONFIG);
                if (File.Exists(file))
                {
                    byte[] bytes = File.ReadAllBytes(file);
                    JsonParse p = new JsonParse(bytes, bytes.Length);
                    JObj jo = (JObj) p.Parse();
                    // this will override values
                    sc.Read(jo, -1);
                }
                else
                {
                    return null;
                }
            }

            // create service instance by reflection
            Type typ = typeof(S);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(ServiceContext)});
            if (ci == null)
            {
                throw new ServiceException(typ + " missing ServiceContext");
            }
            S inst = (S) ci.Invoke(new object[] {sc});
            sc.Work = inst;
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
                    svc.Start();
                }

                Console.WriteLine("ctrl_c to shut down");

                cts.Token.Register(state =>
                    {
                        ((IApplicationLifetime) state).StopApplication();
                        // dispose services
                        foreach (Service svc in services)
                        {
                            svc.OnStop();

                            svc.Dispose();
                        }
                    },
                    Lifetime);

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
                    stopping.Cancel(throwOnFirstException: false);
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
                started.Cancel(throwOnFirstException: false);
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
                stopped.Cancel(throwOnFirstException: false);
            }
            catch (Exception)
            {
            }
        }
    }
}