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
        WebService service;

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

            if (lastConnect < 100)
            {
                // create and run task
                Task.Run(() =>
                {
                    PollEventsAsync();
                });
            }
        }


        ///
        /// A single round of polling for remote events.
        ///
        internal async void PollEventsAsync()
        {
            // schedule

            WebCall call = new WebCall(this);

            // HttpRequestMessage pollRequest = new HttpRequestMessage();
            // client.DefaultRequestHeaders.Add("Range", "");
            // HttpResponseMessage response = await client.SendAsync(pollRequest, HttpCompletionOption.ResponseContentRead);
            // response.Headers.GetValues("lastid");

            byte[] body;

            // if (response.IsSuccessStatusCode)
            // {
            //     byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            //     JsonParse par = new JsonParse(bytes, bytes.Length);
            //     object entity = par.Parse();


            // parse and process evetns
            for (;;)
            {
                long evtid;
                string topic = "";
                DateTime time;

                WebHook hook = null;
                if (service.Hooks.TryGet(topic, out hook))
                {
                    WebEvent evt = new WebEvent
                    {
                        msg = null
                    };
                    hook.Do(evt);
                }
            }
        }
    }
}