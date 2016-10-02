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

        internal MsgPoller(WebService service, string addr)
        {
            this.service = service;
            address = "http://" + addr;
            client = new HttpClient() { BaseAddress = new Uri(address) };
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
                MsgAction a = null;
                if (service.MActions.TryGet("", out a))
                {
                    MsgContext evt = null;
                    a.Do(evt);
                }
            }
        }

    }
}