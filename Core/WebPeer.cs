using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The connect to a remote peer service that the current service depends on.
    ///
    public class WebPeer : WebCaller, IKeyed
    {
        // subdomain name or a reference name
        readonly string name;

        private bool status;

        // tick count
        private int lastConnect;

        public WebPeer(string name, string raddr) : base(raddr)
        {
            this.name = name;
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