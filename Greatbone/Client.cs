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
    public class Client : HttpClient, IKeyable<string>, IPollContext
    {
        const int Ahead = 1000 * 12;
        const string PollAction = "/event";

        //  key for the remote or referenced service 
        readonly string rKey;

        // remote or referenced service name 
        readonly string rName;

        // remote or referenced shard 
        readonly string rShard;

        // the poller task currently running
        Task pollTask;

        Action<IPollContext> poller;

        short interval;

        // point of time to next poll, set because of exception or polling interval
        volatile int retryAt;

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
        /// <param name="rkey">the identifying key for the remote service</param>
        /// <param name="raddr">remote address</param>
        internal Client(string rkey, string raddr)
        {
            rKey = rkey;
            // initialize name and sshard
            if (rkey != null)
            {
                int dash = rkey.LastIndexOf('-');
                if (dash == -1)
                {
                    rName = rkey;
                }
                else
                {
                    rName = rkey.Substring(0, dash);
                    rShard = rkey.Substring(dash + 1);
                }
            }
            BaseAddress = new Uri(raddr);
            Timeout = TimeSpan.FromSeconds(12);
        }

        public Service Service { get; internal set; }

        public string Key => rKey;

        internal void SetPoller(Action<IPollContext> poller, short interval)
        {
            this.poller = poller;
            this.interval = interval;
        }

        public string RefName => rName;

        public string RefShard => rShard;

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
                    Service.WAR("Error in event poller");
                    Service.WAR(e.Message);
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
            if (Service != null)
            {
                req.Headers.TryAddWithoutValidation("X-Caller-Sign", Service.Sign);
                req.Headers.TryAddWithoutValidation("X-Caller-Name", Service.Name);
                req.Headers.TryAddWithoutValidation("X-Caller-Shard", Service.Shard);
            }
            var auth = wc?.Header("Authorization");
            if (auth != null)
            {
                req.Headers.TryAddWithoutValidation("Authorization", auth);
            }
        }

        public string QueryStr { get; set; }

        public async Task<byte[]> PollAsync()
        {
            if (QueryStr == null)
            {
                throw new ServiceException("missing query before event poll");
            }
            string uri = PollAction + "?" + QueryStr;
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, null);
                HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return await resp.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                retryAt = Environment.TickCount + Ahead;
            }
            return null;
        }

        public async Task<M> PollAsync<M>() where M : class, ISource
        {
            if (QueryStr == null)
            {
                throw new ServiceException("missing query before event poll");
            }
            string uri = PollAction + "?" + QueryStr;
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, null);
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
                retryAt = Environment.TickCount + Ahead;
            }
            return null;
        }

        public async Task<D> PollObjectAsync<D>(byte proj = 0x0f) where D : IData, new()
        {
            if (QueryStr == null)
            {
                throw new ServiceException("missing query before event poll");
            }
            string uri = PollAction + "?" + QueryStr;
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, null);
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
                retryAt = Environment.TickCount + Ahead;
            }
            return default;
        }

        public async Task<D[]> PollArrayAsync<D>(byte proj = 0x0f) where D : IData, new()
        {
            if (QueryStr == null)
            {
                throw new ServiceException("missing query before event poll");
            }
            string uri = PollAction + "?" + QueryStr;
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                AddAccessHeaders(req, null);
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
                retryAt = Environment.TickCount + Ahead;
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
                retryAt = Environment.TickCount + Ahead;
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
                retryAt = Environment.TickCount + Ahead;
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
                retryAt = Environment.TickCount + Ahead;
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
                retryAt = Environment.TickCount + Ahead;
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
                retryAt = Environment.TickCount + Ahead;
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
                retryAt = Environment.TickCount + Ahead;
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