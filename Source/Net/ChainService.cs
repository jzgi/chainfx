using System;
using System.Collections.Generic;
using System.Threading;
using CloudUn.Web;

namespace CloudUn.Net
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    public class ChainService : WebService
    {
        List<NetClient> polls = null;

        // the thread schedules and drives periodic jobs, such as event polling 
        Thread scheduler;


        protected internal override void OnCreate()
        {
            // // setup chain net peer references
            // NET = cfg["NET"];
            // if (NET != null)
            // {
            //     for (var i = 0; i < NET.Count; i++)
            //     {
            //         var e = NET.At(i);
            //         peers.Add(new NetClient(e.Key, e.value)
            //         {
            //             Clustered = true
            //         });
            //     }
            // }
            //

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


            CreateWork<AdmlyWork>("admly");
        }
        // inter-node


        // REST Data API
        public void token(WebContext wc)
        {
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