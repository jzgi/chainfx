using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    public class MsgPoller : IKeyed
    {
        readonly WebService service;

        readonly string address;

        HttpClient client;

        private bool status;

        // tick count
        private int lastConnect;

        public string Key => address;

        internal MsgPoller(WebService svc, string addr)
        {
            service = svc;
            address = addr;
        }

        internal MsgPoller()
        {
            client.BaseAddress = new Uri("");

        }

        public void Schedule()
        {
            // check the status


            // create and run task
            Task.Run(() =>
            {
                GetAsync();
            });
        }

        public async void GetAsync()
        {

            client.DefaultRequestHeaders.Add("Range", "");
            HttpResponseMessage resp = await client.GetAsync("");
            if (resp.IsSuccessStatusCode)
            {
                MsgSubscriber sub = null;
                if (service.Subscribers.TryGet("", out sub))
                {
                    MsgContext evt = null;
                    sub.Do(evt); // invoke the handler
                }
            }
        }

    }
}