using System;
using System.Threading;
using SkyChain.Web;

namespace SkyChain.Chain
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    public class ChainService : WebService
    {
        readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        // a bundle of peers
        readonly Map<string, ChainClient> clients = new Map<string, ChainClient>(32);

        // the thread schedules and drives periodic jobs, such as event polling 
        Thread scheduler;


        protected internal override void OnCreate()
        {
            // create and start the scheduler thead
            if (clients != null)
            {
                // to repeatedly check and initiate event polling activities.
                scheduler = new Thread(() =>
                {
                    while (true)
                    {
                        // interval
                        Thread.Sleep(7000);

                        // a scheduling cycle
                        var tick = Environment.TickCount;
                        @lock.EnterReadLock();
                        try
                        {
                            var clis = clients;
                            for (int i = 0; i < clients.Count; i++)
                            {
                                var cli = clis.ValueAt(i);
                                // cli.TryPollAsync(tick);
                            }
                        }
                        finally
                        {
                            @lock.ExitReadLock();
                        }
                    }
                });
                scheduler.Start();
            }
        }


        [Get("Get access token for a login", query: "?id=<-login-id->&password=")]
        [Reply(200, "Success", body: @"{
            name : string, // login name
            id : string, // login id
            token : // access token
        }")]
        [Reply(404, "login not found or invalid password")]
        public void token(WebContext wc)
        {
            string id = wc.Query[nameof(id)];
            string password = wc.Query[nameof(password)];

            using var dc = NewDbContext();

            // retrieve from idents
        }

        public void query(WebContext wc)
        {
        }

        public void querya(WebContext wc)
        {
        }

        public void put(WebContext wc)
        {
        }
    }
}