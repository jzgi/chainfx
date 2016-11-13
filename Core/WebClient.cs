using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// message polling
    ///
    /// remote service call
    ///
    public class WebClient : IKeyed
    {
        // remote address
        readonly string raddr;

        HttpClient client;

        private bool status;

        // tick count
        private int lastConnect;

        internal WebClient(string raddr)
        {
            this.raddr = raddr;
            client = new HttpClient() { BaseAddress = new Uri("http://" + raddr) };
        }

        public string Key => raddr;

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
                JsonParse par = new JsonParse(bytes, bytes.Length);
                object entity = par.Parse();
                MsgHook a = null;
                // if (service.Hooks.TryGet("", out a))
                // {
                //     MsgContext evt = new MsgContext
                //     {
                //         msg = entity
                //     };
                //     a.Do(evt);
                // }
            }
        }

        public async Task<object> GetAsync(string uri)
        {
            HttpRequestMessage msg = new HttpRequestMessage();
            HttpResponseMessage response = await client.SendAsync(msg, HttpCompletionOption.ResponseContentRead);
            response.Headers.GetValues("lastid");

            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            JsonParse par = new JsonParse(bytes, bytes.Length);
            return par.Parse();
        }

        public async Task<HttpResponseMessage> PostXmlAsync(string uri, Action<XmlContent> content)
        {
            XmlContent cont = new XmlContent(true, true);
            content?.Invoke(cont);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new WebCall(cont)
            };

            return await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public async Task<HttpResponseMessage> PostJAsync(string uri, Action<JsonContent> content)
        {
            JsonContent cont = new JsonContent(true, true);
            content?.Invoke(cont);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new WebCall(cont)
            };
            return await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public Elem GetElemAsync(string uri)
        {
            return null;
        }

        public Obj GetJObjAsync(string uri)
        {
            return null;
        }

        public Arr GetJArrAsync(string uri)
        {
            return null;
        }

        public byte[] GetBytesAsync(string uri)
        {
            return null;
        }

    }

}