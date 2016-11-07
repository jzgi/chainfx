using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// message polling
    ///
    /// remote service call
    ///
    public class WebClientHandler : HttpClientHandler
    {
        readonly WebService service;

        // remote address
        readonly string raddr;

        HttpClient client;

        private bool status;

        // tick count
        private int lastConnect;

        internal WebClientHandler(WebService service, string raddr)
        {
            this.service = service;
            this.raddr = raddr;
            client = new HttpClient() { BaseAddress = new Uri("http://" + raddr) };
        }

        public string Key => raddr;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return null;
        }

    }

}