using System;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Greatbone.Core
{
    public class Application
    {
        const string CfgFile = "$web.json";

        static readonly ServiceException CfgEx = new ServiceException("error loading " + CfgFile);

        internal static readonly Lifetime Lifetime = new Lifetime();

        static readonly List<Service> Services = new List<Service>(8);


        public static S Create<S>(string name, JObj cfgjo = null, CheckAttribute[] checks = null, UiAttribute ui = null) where S : Service
        {
            if (cfgjo == null) // need to load configuration file
            {
                if (File.Exists(CfgFile))
                {
                    byte[] bytes = File.ReadAllBytes(CfgFile);
                    JsonParse p = new JsonParse(bytes, bytes.Length);
                    cfgjo = (JObj)p.Parse();
                }
                else
                {
                    return null;
                }
            }

            // create instance by reflection
            Type typ = typeof(S);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(FolderContext) });
            if (ci == null)
            {
                throw new ServiceException(typ + " missing FolderContext");
            }
            FolderContext fc = new FolderContext(name)
            {
                Checks = checks,
                Ui = ui,
                IsVar = false,
                Parent = null,
                Level = 0,
                Directory = name,
                Config = cfgjo
            };
            S service = (S)ci.Invoke(new object[] { fc });
            Services.Add(service);
            return service;
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
                foreach (Service svc in Services)
                {
                    svc.Start();
                }

                Console.WriteLine("ctrl_c to shut down");

                cts.Token.Register(state =>
                    {
                        ((IApplicationLifetime)state).StopApplication();
                        // dispose services
                        foreach (Service svc in Services)
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

    class Lifetime : IApplicationLifetime
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
                    // TODO: LOG
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
                // TODO: LOG
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
                // TODO: LOG
            }
        }
    }
}