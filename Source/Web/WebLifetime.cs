using System;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ChainFX.Web
{
    internal class WebLifetime : IHostApplicationLifetime
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