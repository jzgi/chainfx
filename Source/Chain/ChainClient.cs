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
        const int REQUEST_TIMEOUT = 3;

        const short
            NORMAL = 1,
            NETWORK_ERROR = 2,
            DATA_ERROR = 3,
            PEER_ERROR = 4;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, null},
            {NORMAL, "正常"},
            {NETWORK_ERROR, "网络错误"},
            {DATA_ERROR, "数据错误"},
            {PEER_ERROR, "对伴错误"},
        };

        // when a chain connector
        readonly Peer info;

        // acceptable remote addresses
        readonly IPAddress[] addrs;

        Task<(short, Arch[])> task;

        // the lastest http response status, 
        short status;


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
            Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT);
        }

        public short Key => info.id;

        public Peer Info => info;

        ///
        /// <returns>0) void, 1) remoting, 200) ok, 204) no content, 500) server error</returns>
        /// 
        public short TryReap(out Arch[] block)
        {
            block = null;
            if (task == null)
            {
                return 0;
            }
            if (!task.IsCompleted)
            {
                return 1;
            }
            var (code, cnt) = task.Result;

            if (code == 500)
            {
                status = PEER_ERROR;
            }
            else if (code == 200 || code == 204)
            {
                status = NORMAL;
            }

            block = cnt;
            return code;
        }

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

        internal void SetDataError(short seq)
        {
            status = DATA_ERROR;
            // this.seq = seq;
        }

        public bool IsCompleted => task.IsCompleted;

        public void StartRemote(int blockid)
        {
            if (status != DATA_ERROR)
            {
                // schedule to execute
                if (task == null || task.IsCompleted)
                {
                    (task = RemoteAsync(blockid)).Start();
                }
            }
        }

        async Task<(short, Arch[])> RemoteAsync(int blockid)
        {
            const string uri = "/onimport";
            try
            {
                // request
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                req.Headers.TryAddWithoutValidation(IChain.X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_BLOCK_ID, blockid.ToString());

                // response
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return ((short) rsp.StatusCode, null);
                }
                var hdrs = rsp.Content.Headers;

                // block properties
                // string ctyp = hdrs.GetValue("Content-Type");
                // var x_seq = hdrs.GetValue(IChain.X_SEQ);
                // var x_stamp = hdrs.GetValue(IChain.X_STAMP);
                // var x_digest = hdrs.GetValue(IChain.X_DIGEST);
                // var x_prev_digest = hdrs.GetValue(IChain.X_PREV_DIGEST);

                var bytea = await rsp.Content.ReadAsByteArrayAsync();
                var arr = (JArr) new JsonParser(bytea, bytea.Length).Parse();
                var dat = arr.ToArray<Arch>();
                return (200, dat);
            }
            catch
            {
                status = NETWORK_ERROR;
                return (0, null);
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
                req.Headers.TryAddWithoutValidation(IChain.X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_JOB, job.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_STEP, step.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_ACCOUNT, acct);
                req.Headers.TryAddWithoutValidation(IChain.X_NAME, name);
                req.Headers.TryAddWithoutValidation(IChain.X_LEDGER, ldgr);

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    return (short) rsp.StatusCode;
                }

                return (short) rsp.StatusCode;
            }
            catch
            {
                // retryAt = Environment.TickCount + AHEAD;
            }

            return 500;
        }

        public async Task<short> CallJobBackwardAsync(long job, short step)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "/onback");
                req.Headers.TryAddWithoutValidation(IChain.X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_JOB, job.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_STEP, step.ToString());

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    return (short) rsp.StatusCode;
                }

                return (short) rsp.StatusCode;
            }
            catch
            {
                // retryAt = Environment.TickCount + AHEAD;
            }

            return 500;
        }

        public async Task<short> CallJobEndAsync(long job, short step)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "/onback");
                req.Headers.TryAddWithoutValidation(IChain.X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_JOB, job.ToString());
                req.Headers.TryAddWithoutValidation(IChain.X_STEP, step.ToString());

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    return (short) rsp.StatusCode;
                }

                return (short) rsp.StatusCode;
            }
            catch
            {
                // retryAt = Environment.TickCount + AHEAD;
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
                // retryAt = Environment.TickCount + AHEAD;
            }

            return (500, null);
        }
    }
}