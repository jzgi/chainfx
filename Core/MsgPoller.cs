using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    public class MsgPoller : IMember
    {
        WebService service;

        string address;

        HttpClient client;

        private bool status;

        // tick count
        private int lastConnect;

        public string Key => address;


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
                MsgSubscribe sub = null;
                if (service.Subscribes.TryGet("", out sub))
                {
                    MsgEvent evt = null;
                    sub.Do(evt); // invoke the handler
                }
            }
        }

    }
}