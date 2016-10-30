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


        public async void PollAsync()
        {

            HttpRequestMessage pollRequest = new HttpRequestMessage();
            client.DefaultRequestHeaders.Add("Range", "");
            HttpResponseMessage response = await client.SendAsync(pollRequest, HttpCompletionOption.ResponseContentRead);
            response.Headers.GetValues("lastid");

            if (response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                JParse par = new JParse(bytes, bytes.Length);
                object entity = par.Parse();
                MsgHook a = null;
                if (service.Hooks.TryGet("", out a))
                {
                    MsgContext evt = new MsgContext
                    {
                        msg = entity
                    };
                    a.Do(evt);
                }
            }
        }

        public async Task<object> GetAsync(string uri)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            HttpResponseMessage response = await client.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
            response.Headers.GetValues("lastid");

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            JParse par = new JParse(bytes, bytes.Length);
            return par.Parse();
        }

        public async Task<object> PostAsync(string uri, object content)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            HttpResponseMessage response = await client.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
            response.Headers.GetValues("lastid");

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            JParse par = new JParse(bytes, bytes.Length);
            return par.Parse();
        }

    }
}