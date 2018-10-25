using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Greatbone.DataUtility;

namespace Greatbone
{
    /// <summary>
    /// A client connector that implements both one-to-one and one-to-many communication in both sync and async approaches.
    /// </summary>
    public class Client : HttpClient, IKeyable<string>, IEventContext
    {
        const int Ahead = 1000 * 12;
        const string PollAction = "/event";

        // remote service name 
        readonly string svcname;

        private short idx;

        // the poller task currently running
        Task pollTask;

        string pollQuery;

        Action<IEventContext> poller;

        short interval;

        // point of time to next poll, set because of exception or polling interval
        volatile int retryPt;

        /// <summary>
        /// Used to construct a secure client by passing handler with certificate.
        /// </summary>
        /// <param name="handler"></param>
        public Client(HttpClientHandler handler) : base(handler)
        {
        }

        /// <summary>
        /// Used to construct a random client that does not necessarily connect to a remote service. 
        /// </summary>
        /// <param name="raddr"></param>
        public Client(string raddr) : this(null, raddr)
        {
        }

        /// <summary>
        /// Used to construct a service client. 
        /// </summary>
        /// <param name="key">remote service key</param>
        /// <param name="addr">remote address</param>
        internal Client(string key, string addr)
        {
            if (key != null)
            {
                int dash = key.LastIndexOf('-');
                if (dash == -1)
                {
                    this.svcname = key;
                }
                else
                {
                    this.svcname = key.Substring(0, dash);
                    string sub = key.Substring(dash + 1);
                    short.TryParse(sub, out idx);
                }
            }
            BaseAddress = new Uri(addr);
            Timeout = TimeSpan.FromSeconds(12);
        }

        public Service Service { get; internal set; }

        public string Key => svcname;

        internal void SetPoller(Action<IEventContext> poller, short interval)
        {
            this.poller = poller;
            this.interval = interval;
        }

        public bool FirstTime { get; }
        public string RemoteSvc { get; }

        public void SetParam(string name, string v)
        {
            throw new NotImplementedException();
        }

        public string GetAsync()
        {
            throw new NotImplementedException();
        }

        internal async void TryPollAsync(int ticks)
        {
            if (ticks < retryPt)
            {
                return;
            }
            if (pollTask != null && !pollTask.IsCompleted)
            {
                return;
            }
            await (pollTask = new Task(() =>
            {
                try
                {
                    // execute an event poll/process cycle
                    poller(this);
                }
                catch
                {
                }
                finally
                {
                    retryPt += interval * 1000;
                }
            }));
        }

        //
        // RPC
        //

        void AddAccessHeaders(HttpRequestMessage req, WebContext wc)
        {
            if (Service != null)
            {
                req.Headers.TryAddWithoutValidation("X-Sign", Service.Sign);
                req.Headers.TryAddWithoutValidation("X-Service", Service.Key);
                req.Headers.TryAddWithoutValidation("X-Shard", Service.Shard);
            }
            var auth = wc?.Header("Authorization");
            if (auth != null)
            {
                req.Headers.TryAddWithoutValidation("Authorization", auth);
            }
        }

        public async Task<byte[]> PollAsync(string query)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, PollAction + "?" + query);
                AddAccessHeaders(req, null);

                HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return await resp.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                retryPt = Environment.TickCount + Ahead;
            }
            return null;
        }

        public async Task<byte[]> GetAsync(string uri, WebContext wc = null)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return await resp.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                retryPt = Environment.TickCount + Ahead;
            }
            return null;
        }

        public async Task<M> GetAsync<M>(string uri, WebContext wc) where M : class, ISource
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
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
                retryPt = Environment.TickCount + Ahead;
            }
            return null;
        }

        public async Task<D> GetObjectAsync<D>(string uri, byte proj = 0x0f, WebContext wc = null) where D : IData, new()
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
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
                retryPt = Environment.TickCount + Ahead;
            }
            return default;
        }

        public async Task<D[]> GetArrayAsync<D>(string uri, byte proj = 0x0f, WebContext wc = null) where D : IData, new()
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, wc);
                HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
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
                retryPt = Environment.TickCount + Ahead;
            }
            return null;
        }

        public async Task<int> PostAsync(string uri, IContent content, WebContext wc = null)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
                AddAccessHeaders(req, wc);
                req.Content = (HttpContent) content;
                req.Headers.TryAddWithoutValidation("Content-Type", content.Type);
                req.Headers.TryAddWithoutValidation("Content-Length", content.Size.ToString());

                HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return (int) rsp.StatusCode;
            }
            catch
            {
                retryPt = Environment.TickCount + Ahead;
            }
            finally
            {
                if (content is DynamicContent cont)
                {
                    BufferUtility.Return(cont);
                }
            }
            return 0;
        }

        public async Task<(int, M)> PostAsync<M>(string uri, IContent content, string token = null) where M : class, ISource
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
                if (token != null)
                {
                    req.Headers.Add("Authorization", "Token " + token);
                }
                req.Content = (HttpContent) content;
                req.Headers.TryAddWithoutValidation("Content-Type", content.Type);
                req.Headers.TryAddWithoutValidation("Content-Length", content.Size.ToString());

                HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                string ctyp = rsp.Content.Headers.GetValue("Content-Type");
                if (ctyp == null)
                {
                    return ((int) rsp.StatusCode, null);
                }
                else
                {
                    byte[] bytes = await rsp.Content.ReadAsByteArrayAsync();
                    M src = ParseContent(ctyp, bytes, bytes.Length, typeof(M)) as M;
                    return ((int) rsp.StatusCode, src);
                }
            }
            catch
            {
                retryPt = Environment.TickCount + Ahead;
            }
            finally
            {
                if (content is DynamicContent cont)
                {
                    BufferUtility.Return(cont);
                }
            }
            return default;
        }
    }
}