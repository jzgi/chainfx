using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The connect to a remote peer service that the current service depends on.
    ///
    public class WebClient : HttpClient, IRollable
    {
        WebService service;

        // subdomain name or a reference name
        readonly string name;

        private bool status;

        // tick count
        private int lastConnect;


        // the event polling context for this remote 
        WebClientContext pollctx;

        public WebClient(string name, string raddr)
        {
            this.name = name;
            string addr = raddr.StartsWith("http") ? raddr : "http://" + raddr;
            BaseAddress = new Uri(addr);
        }

        public string Name => name;

        internal void Schedule()
        {
            // check the status

            if (lastConnect < 100)
            {
                // create and run task
                Task.Run(() =>
                {
                    PollAsync();
                });
            }
        }


        internal async void PollAsync()
        {
            // parse and process evetns
            for (;;)
            {
                long id;
                string name = "";
                DateTime time;
                WebEvent hook = null;
                if (service.Events.TryGet(name, out hook))
                {
                    WebEventContext ec = new WebEventContext
                    {
                        // body = null
                    };
                    hook.Do(ec);
                }
            }
        }

        public WebClientContext NewContext()
        {
            return null;
        }

    }
}