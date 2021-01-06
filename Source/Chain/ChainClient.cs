using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Chain
{
    /// <summary>
    /// A chain client connector for a specific remote peer..
    /// </summary>
    public class ChainClient : HttpClient, IKeyable<short>
    {
        const int TIMEOUT_SECONDS = 3;

        const int AHEAD = 1000 * 12;

        // when a chain connector
        readonly Peer info;

        // acceptable remote addresses
        readonly IPAddress[] addrs;

        // the lastest status, aligned to HTTP specs
        HttpStatusCode status;

        // the lastest result
        Block block;

        BlockOp[] records;

        // point of time to next poll, set because of exception or polling interval
        volatile int retryAt;

        int lastseq;

        Task task;


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

        public short Key => info.id;

        public Peer Info => info;

        public (Block, BlockOp[]) Result => (block, records);

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

        public void StartPoll(int ticks)
        {
            // reset
            block = null;
            records = null;

            // schedule to execute
            (task = PollAsync()).Start();
        }

        async Task PollAsync()
        {
            string uri = "/onpoll-" + lastseq;
            try
            {
                // request
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                req.Headers.TryAddWithoutValidation("X-From", ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation("X-From", ChainEnviron.Info.id.ToString());


                // response
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                status = rsp.StatusCode;
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }
                var hdrs = rsp.Content.Headers;

                // block properties
                string ctyp = hdrs.GetValue("Content-Type");
                var x_seq = hdrs.GetValue(Chain.X_SEQ);
                var x_stamp = hdrs.GetValue(Chain.X_STAMP);
                var x_digest = hdrs.GetValue(Chain.X_DIGEST);
                var x_prev_digest = hdrs.GetValue(Chain.X_PREV_DIGEST);


                // block & records
                block = new Block
                {
                    peerid = 0,
                    seq = x_seq.ToInt(),
                    stamp = x_stamp.ToDateTime(),
                    dgst = x_digest.ToLong(),
                    pdgst = x_prev_digest.ToLong(),
                };
                byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                var arr = (JArr) new JsonParser(bytea, bytea.Length).Parse();
                records = arr.ToArray<BlockOp>();
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }
        }

        //
        // RPC
        //

        public async Task<short> CallJobForwardAsync(long job, short step, string acct, string name, string ldgr)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "/onforth");
                req.Headers.TryAddWithoutValidation(Chain.X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(Chain.X_JOB, job.ToString());
                req.Headers.TryAddWithoutValidation(Chain.X_STEP, step.ToString());
                req.Headers.TryAddWithoutValidation(Chain.X_ACCOUNT, acct);
                req.Headers.TryAddWithoutValidation(Chain.X_NAME, name);
                req.Headers.TryAddWithoutValidation(Chain.X_LEDGER, ldgr);

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    return (short) rsp.StatusCode;
                }

                return (short) rsp.StatusCode;
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return 500;
        }

        public async Task<short> CallJobBackwardAsync(long job, short step)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "/onback");
                req.Headers.TryAddWithoutValidation(Chain.X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(Chain.X_JOB, job.ToString());
                req.Headers.TryAddWithoutValidation(Chain.X_STEP, step.ToString());

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    return (short) rsp.StatusCode;
                }

                return (short) rsp.StatusCode;
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return 500;
        }

        public async Task<short> CallJobEndAsync(long job, short step)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "/onback");
                req.Headers.TryAddWithoutValidation(Chain.X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(Chain.X_JOB, job.ToString());
                req.Headers.TryAddWithoutValidation(Chain.X_STEP, step.ToString());

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    return (short) rsp.StatusCode;
                }

                return (short) rsp.StatusCode;
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return 500;
        }


        void AddAccessHeaders(HttpRequestMessage req, WebContext wc)
        {
            var cfg = Framework.webcfg;
            req.Headers.TryAddWithoutValidation("X-From", ChainEnviron.Info.id.ToString());

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