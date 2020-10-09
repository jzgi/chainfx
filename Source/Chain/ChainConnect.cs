using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Skyiah.Web;
using static Skyiah.DataUtility;

namespace Skyiah.Chain
{
    /// <summary>
    /// A client connector that implements both one-to-one and one-to-many communication in both sync and async approaches.
    /// </summary>
    public class ChainConnect : HttpClient, IKeyable<string>, IPollContext
    {
        const int AHEAD = 1000 * 12;

        const string POLL_ACTION = "/event";

        //  key for the remote or referenced service 
        readonly string key;

        // remote or referenced service name 
        readonly string name;

        // remote or referenced shard 
        readonly string addr;

        // the poller task currently running
        Task pollTask;

        Action<IPollContext> poller;

        short interval;

        // remote crypto
        long crypto;

        // remote client addresses
        IPAddress[] addrs;


        // point of time to next poll, set because of exception or polling interval
        volatile int retryAt;

        /// <summary>
        /// Used to construct a random client that does not necessarily connect to a remote service. 
        /// </summary>
        /// <param name="addr"></param>
        public ChainConnect(string addr)
        {
            this.addr = addr;

            BaseAddress = new Uri(addr);
            Timeout = TimeSpan.FromSeconds(12);
        }

        public ChainConnect(HttpClientHandler handler, string key = null, string name = null, string addr = null) : base(handler)
        {
            this.key = key;
            this.name = name;
            this.addr = addr;

            if (addr != null)
            {
                BaseAddress = new Uri(addr);
            }

            Timeout = TimeSpan.FromSeconds(12);
        }

        public string Key => key;

        internal void SetPoller(Action<IPollContext> poller, short interval)
        {
            this.poller = poller;
            this.interval = interval;
        }

        public string RefName => name;

        public string RefShard => addr;

        internal async void TryPollAsync(int ticks)
        {
            if (ticks < retryAt)
            {
                return;
            }

            if (pollTask != null && !pollTask.IsCompleted)
            {
                return;
            }

            await (pollTask = Task.Run(() =>
            {
                try
                {
                    // execute an event poll/process cycle
                    poller(this);
                }
                catch (Exception e)
                {
                    Framework.WAR("Error in event poller");
                    Framework.WAR(e.Message);
                }
                finally
                {
                    retryAt += interval * 1000;
                }
            }));
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