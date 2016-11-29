using System;
using System.Net.Http;

namespace Greatbone.Core
{
    ///
    /// A connector to a remote web server, that can send requests and receive responses.
    ///
    public class WebClient : HttpClient
    {
        public WebClient(string raddr)
        {
            string addr = raddr.StartsWith("http") ? raddr : "http://" + raddr;
            BaseAddress = new Uri(addr);
        }
    }
}