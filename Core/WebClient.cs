using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// message polling
    ///
    /// remote service call
    ///
    class WebClient : IKeyed
    {
        readonly WebService service;

        readonly string addr;

        HttpClient client;

        private bool status;

        // tick count
        private int lastConnect;

        internal WebClient(WebService service, string addr)
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
                PollAsync();
            });
        }

        HttpRequestMessage pollRequest = new HttpRequestMessage();

        public async void PollAsync()
        {

            client.DefaultRequestHeaders.Add("Range", "");
            HttpResponseMessage response = await client.SendAsync(pollRequest, HttpCompletionOption.ResponseContentRead);
            response.Headers.GetValues("lastid");

            if (response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                JParse parse = new JParse(bytes, bytes.Length);
                object entity = parse.Parse();
                MsgHook a = null;
                if (service.MsgActions.TryGet("", out a))
                {
                    MsgContext evt = new MsgContext
                    {
                        msg = entity
                    };
                    a.Handle(evt);
                }
            }
        }

        public async Task<object> CallAsync(string service, string part)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            HttpResponseMessage response = await client.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
            response.Headers.GetValues("lastid");

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            JParse parse = new JParse(bytes, bytes.Length);
            return parse.Parse();
        }

    }
}