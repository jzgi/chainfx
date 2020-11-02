using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Chain
{
    /// <summary>
    /// A chain client connector targeted to a specific peer..
    /// </summary>
    public class ChainClient : HttpClient, IKeyable<string>
    {
        const int TIMEOUT_SECONDS = 12;

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

        Record[] blockrecs;


        /// <summary>
        /// To construct a chain client. 
        /// </summary>
        internal ChainClient(Peer info, ChainClientHandler handler = null) : base(handler ?? new ChainClientHandler())
        {
            this.info = info;
            var baseuri = info.uri;

            try
            {
                addrs = Dns.GetHostAddresses(baseuri);
                BaseAddress = new Uri(baseuri + "/poll");
            }
            catch (Exception e)
            {
            }
            Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
        }

        // point of time to next poll, set because of exception or polling interval
        volatile int retryAt;


        public Peer Info => info;

        public string Key => info?.name;

        public short Status
        {
            get
            {
                lock (this) return status;
            }
        }

        public Block Result
        {
            get
            {
                lock (this) return block;
            }
        }


        internal async void TryPollAsync(int ticks)
        {
            if (ticks < retryAt)
            {
                return;
            }

            lock (this)
            {
            }

            string uri = POLL_ACTION + "?" + QueryString;
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

                // block content
                byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();

                JArr arr = (JArr) new JsonParser(bytea, bytea.Length).Parse();
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
            // var cfg = Framework.Config;
//                req.Headers.TryAddWithoutValidation("X-Caller-Sign", Framework.sign);
//                req.Headers.TryAddWithoutValidation("X-Caller-Name", cfg.name);
//                req.Headers.TryAddWithoutValidation("X-Caller-Shard", cfg.shard);

            var auth = wc?.Header("Authorization");
            if (auth != null)
            {
                req.Headers.TryAddWithoutValidation("Authorization", auth);
            }
        }

        public string QueryString { get; set; }

        public async Task<byte[]> PollAsync()
        {
            if (QueryString == null)
            {
                throw new FrameworkException("missing query before event poll");
            }

            string uri = POLL_ACTION + "?" + QueryString;
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, null);
                var resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return await resp.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return null;
        }

        public Task<M> PollAsync<M>() where M : class, ISource
        {
            throw new NotImplementedException();
        }

        public Task<D> PollObjectAsync<D>(byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public Task<D[]> PollArrayAsync<D>(byte proj = 15) where D : IData, new()
        {
            throw new NotImplementedException();
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