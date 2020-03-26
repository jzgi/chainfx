using System.Net;
using System.Net.Http;

namespace CloudUn.Net
{
    public class ChainPeer : NetClient
    {
        // remote crypto
        long crypto;

        // remote client addresses
        IPAddress[] addrs;


        public ChainPeer(HttpClientHandler handler) : base(handler)
        {
        }

        public ChainPeer(string raddr) : base(raddr)
        {
        }

        internal ChainPeer(string rkey, string raddr) : base(rkey, raddr)
        {
        }
    }
}