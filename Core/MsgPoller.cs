using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    public class MsgPoller : IKeyed
    {
        readonly WebService service;

        readonly string addr;

        HttpClient client;

        private bool status;

        // tick count
        private int lastConnect;

        internal MsgPoller(WebService service, string addr)
        {
            this.service = service;
            this.addr = addr;
            client = new HttpClient() { BaseAddress = new Uri("http://" + addr) };
        }

        public string Key => addr;

        internal void Schedule()
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
                if (service.MsgActions.TryGet("", out a))
                {
                    MsgContext evt = null;
                    a.Handle(evt);
                }
            }
        }

    }
}