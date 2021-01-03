using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Chain
{
    /// <summary>
    /// A chain client connector targeted to a specific remote peer..
    /// </summary>
    public class ChainClient : HttpClient, IKeyable<string>
    {
        const int TIMEOUT_SECONDS = 5;

        const int AHEAD = 1000 * 12;

        const string POLL_ACTION = "/event";

        // when a chain connector
        readonly Peer info;

        // acceptable remote addresses
        readonly IPAddress[] addrs;

        // the lastest status
        short status;

        // the lastest result
        Block block;

        State[] states;


        /// <summary>
        /// To construct a chain client. 
        /// </summary>
        internal ChainClient(Peer info, ChainClientHandler handler = null) : base(handler ?? new ChainClientHandler())
        {
            this.info = info;
            var baseuri = info.uri;
            BaseAddress = new Uri(baseuri);
            try
            {
                addrs = Dns.GetHostAddresses(BaseAddress.Host);
            }
            catch (Exception e)
            {
            }
            Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
        }

        // point of time to next poll, set because of exception or polling interval
        volatile int retryAt;

        int lastseq;

        Task task;

        public string Key => info?.name;

        public Peer Info => info;

        public (Block, State[]) Result => (block, states);

        public bool IsRemoteAddr(IPAddress addr)
        {
            for (int i = 0; i < addrs.Length; i++)
            {
                if (addrs[i].Equals(addr))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCompleted => task.IsCompleted;

        public void Start()
        {
            block = null;
            states = null;
            task = PollAsync(2);
            task.Start();
        }

        internal async Task PollAsync(int ticks)
        {
            if (ticks < retryAt)
            {
                return;
            }

            lock (this)
            {
            }

            string uri = "/block-" + lastseq;
            try
            {
                // request
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, null);
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                // response
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }
                var hdrs = rsp.Content.Headers;

                // block properties
                string ctyp = hdrs.GetValue("Content-Type");

                block = new Block
                {
                    peerid = null,
                    seq = 0,
                };

                // block content
                byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                var arr = (JArr) new JsonParser(bytea, bytea.Length).Parse();
                states = arr.ToArray<State>();

                int len = arr.Count;
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }
        }

        //
        // RPC
        //

        void AddAccessHeaders(HttpRequestMessage req, WebContext wc)
        {
            var cfg = Framework.webcfg;
            req.Headers.TryAddWithoutValidation("X-From", ChainEnviron.Info.id);

            // var auth = wc?.Header("Authorization");
            // if (auth != null)
            // {
            //     req.Headers.TryAddWithoutValidation("Authorization", auth);
            // }
        }

        public async Task<(short, byte[])> GetAsync(string uri, WebContext wc = null)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    return ((short) rsp.StatusCode, await rsp.Content.ReadAsByteArrayAsync());
                }

                return ((short) rsp.StatusCode, null);
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return (500, null);
        }
    }
}