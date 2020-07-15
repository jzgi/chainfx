using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SkyCloud.Chain;
using static SkyCloud.DataUtility;

namespace SkyCloud.Web
{
    /// <summary>
    /// A client connector that implements both one-to-one and one-to-many communication in both sync and async approaches.
    /// </summary>
    public class WebConnect : HttpClient, IKeyable<string>, IPollContext
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
        public WebConnect(string addr)
        {
            this.addr = addr;

            BaseAddress = new Uri(addr);
            Timeout = TimeSpan.FromSeconds(12);
        }

        public WebConnect(HttpClientHandler handler, string key = null, string name = null, string addr = null) : base(handler)
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

        public async Task<M> PollAsync<M>() where M : class, ISource
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
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                return (M) ParseContent(ctyp, bytea, bytea.Length, typeof(M));
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return null;
        }

        public async Task<D> PollObjectAsync<D>(byte proj = 0x0f) where D : IData, new()
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
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return default;
                }

                byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                ISource inp = ParseContent(ctyp, bytea, bytea.Length);
                D obj = new D();
                obj.Read(inp, proj);
                return obj;
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return default;
        }

        public async Task<D[]> PollArrayAsync<D>(byte proj = 0x0f) where D : IData, new()
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
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                ISource inp = ParseContent(ctyp, bytea, bytea.Length);
                return inp.ToArray<D>(proj);
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return null;
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

        public async Task<(short, M)> GetAsync<M>(string uri, WebContext wc) where M : class, ISource
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                    string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                    var model = (M) ParseContent(ctyp, bytea, bytea.Length, typeof(M));
                    return ((short) rsp.StatusCode, model);
                }

                return ((short) rsp.StatusCode, null);
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return (500, null);
        }

        public async Task<(short, D)> GetObjectAsync<D>(string uri, byte proj = 0x0f, WebContext wc = null) where D : IData, new()
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                    string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                    ISource inp = ParseContent(ctyp, bytea, bytea.Length);
                    D obj = new D();
                    obj.Read(inp, proj);
                    return ((short) rsp.StatusCode, obj);
                }

                return ((short) rsp.StatusCode, default);
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return (500, default);
        }

        public async Task<(short, D[])> GetArrayAsync<D>(string uri, byte proj = 0x0f, WebContext wc = null) where D : IData, new()
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
                    string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                    ISource inp = ParseContent(ctyp, bytea, bytea.Length);
                    var arr = inp.ToArray<D>(proj);
                    return ((short) rsp.StatusCode, arr);
                }

                return ((short) rsp.StatusCode, null);
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }

            return (500, null);
        }

        public async Task<short> PostAsync(string uri, IContent content, WebContext wc = null)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, uri);
                AddAccessHeaders(req, wc);
                req.Content = (HttpContent) content;
                req.Headers.TryAddWithoutValidation("Content-Type", content.Type);
                req.Headers.TryAddWithoutValidation("Content-Length", content.Count.ToString());

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return (short) rsp.StatusCode;
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }
            finally
            {
                if (content is DynamicContent cnt)
                {
                    ArrayUtility.Return(cnt.Buffer);
                }
            }

            return 0;
        }

        public async Task<(short, M)> PostAsync<M>(string uri, IContent content, string token = null) where M : class, ISource
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, uri);
                if (token != null)
                {
                    req.Headers.Add("Authorization", "Token " + token);
                }

                req.Content = (HttpContent) content;
                req.Headers.TryAddWithoutValidation("Content-Type", content.Type);
                req.Headers.TryAddWithoutValidation("Content-Length", content.Count.ToString());

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                if (ctyp == null)
                {
                    return ((short) rsp.StatusCode, null);
                }
                else
                {
                    byte[] bytes = await rsp.Content.ReadAsByteArrayAsync();
                    M src = ParseContent(ctyp, bytes, bytes.Length, typeof(M)) as M;
                    return ((short) rsp.StatusCode, src);
                }
            }
            catch
            {
                retryAt = Environment.TickCount + AHEAD;
            }
            finally
            {
                if (content is DynamicContent cnt)
                {
                    ArrayUtility.Return(cnt.Buffer);
                }
            }

            return default;
        }
    }
}