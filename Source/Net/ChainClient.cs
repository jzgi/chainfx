using System.Net.Http;

namespace CloudUn.Net
{
    public class ChainClient : NetClient
    {
        public ChainClient(HttpClientHandler handler) : base(handler)
        {
        }

        public ChainClient(string raddr) : base(raddr)
        {
        }

        internal ChainClient(string rkey, string raddr) : base(rkey, raddr)
        {
        }
    }
}