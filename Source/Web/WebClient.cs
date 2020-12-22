using System;
using System.Net.Http;
using System.Threading.Tasks;
using static SkyChain.DataUtility;

namespace SkyChain.Web
{
    /// <summary>
    /// A client connector that implements both one-to-one and one-to-many communication in both sync and async approaches.
    /// </summary>
    public class WebClient : HttpClient
    {
        const int AHEAD = 1000 * 12;

        // remote or referenced shard 
        readonly string baseuri;

        // point of time to next poll, set because of exception or polling interval
        volatile int retryAt;

        /// <summary>
        /// Used to construct a random client that does not necessarily connect to a remote service. 
        /// </summary>
        public WebClient(string baseuri, WebClientHandler handler = null) : base(handler ?? new WebClientHandler())
        {
            this.baseuri = baseuri;

            BaseAddress = new Uri(baseuri);
            Timeout = TimeSpan.FromSeconds(12);
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

        public async Task<(short, M)> GetAsync<M>(string uri, string authstr = null) where M : class, ISource
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (authstr != null)
                {
                    req.Headers.TryAddWithoutValidation("Authorization", authstr);
                }
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
                    cnt.Clear();
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
                    cnt.Clear();
                }
            }

            return default;
        }
    }
}