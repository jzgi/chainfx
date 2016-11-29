using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The connect to a remote peer service that the current service depends on.
    ///
    public class WebClient : HttpClient, IKeyed
    {
        // subdomain name or a reference name
        readonly string name;

        private bool status;

        // tick count
        private int lastConnect;

        public WebClient(string name, string raddr) 
        {
            this.name = name;
            string addr = raddr.StartsWith("http") ? raddr : "http://" + raddr;
            BaseAddress = new Uri(addr);
        }

        public string Key => name;

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

            // HttpRequestMessage pollRequest = new HttpRequestMessage();
            // client.DefaultRequestHeaders.Add("Range", "");
            // HttpResponseMessage response = await client.SendAsync(pollRequest, HttpCompletionOption.ResponseContentRead);
            // response.Headers.GetValues("lastid");

            // if (response.IsSuccessStatusCode)
            // {
            //     byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            //     JsonParse par = new JsonParse(bytes, bytes.Length);
            //     object entity = par.Parse();
            //     WebHook a = null;
            //     // if (service.Hooks.TryGet("", out a))
            //     // {
            //     //     MsgContext evt = new MsgContext
            //     //     {
            //     //         msg = entity
            //     //     };
            //     //     a.Do(evt);
            //     // }
            // }
        }
    }
}