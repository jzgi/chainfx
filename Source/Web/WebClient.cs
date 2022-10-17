using System;
using System.Net.Http;
using System.Threading.Tasks;
using static ChainFx.DataUtility;

namespace ChainFx.Web
{
    /// <summary>
    /// A client connector that implements both one-to-one and one-to-many communication in both sync and async approaches.
    /// </summary>
    public class WebClient : HttpClient
    {
        const string
            CONTENT_TYPE = "Content-Type",
            CONTENT_LENGTH = "Content-Length",
            AUTHORIZATION = "Authorization";

        /// <summary>
        /// Used to construct a random client that does not necessarily connect to a remote service. 
        /// </summary>
        public WebClient(string baseUri, WebClientHandler handler = null) : base(handler ?? new WebClientHandler())
        {
            BaseAddress = new Uri(baseUri);
            Timeout = TimeSpan.FromSeconds(7);
        }

        //
        // RPC
        //

        public async Task<(short, S)> GetAsync<S>(string uri, string authstring = null) where S : class, ISource
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (authstring != null)
                {
                    req.Headers.TryAddWithoutValidation(AUTHORIZATION, authstring);
                }
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    var bytea = await rsp.Content.ReadAsByteArrayAsync();
                    string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                    var model = (S) ParseContent(ctyp, bytea, bytea.Length, typeof(S));
                    return ((short) rsp.StatusCode, model);
                }
                return ((short) rsp.StatusCode, default);
            }
            catch
            {
                return (0, default);
            }
        }

        public async Task<(short, D)> GetObjectAsync<D>(string uri, short proj = 0xff, string authstring = null) where D : IData, new()
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (authstring != null)
                {
                    req.Headers.TryAddWithoutValidation(AUTHORIZATION, authstring);
                }
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    var bytea = await rsp.Content.ReadAsByteArrayAsync();
                    string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                    var inp = ParseContent(ctyp, bytea, bytea.Length);
                    var obj = new D();
                    obj.Read(inp, proj);
                    return ((short) rsp.StatusCode, obj);
                }
                return ((short) rsp.StatusCode, default);
            }
            catch
            {
                return (0, default);
            }
        }

        public async Task<(short, D[])> GetArrayAsync<D>(string uri, short proj = 0xff, string authstring = null) where D : IData, new()
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (authstring != null)
                {
                    req.Headers.TryAddWithoutValidation(AUTHORIZATION, authstring);
                }
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.IsSuccessStatusCode)
                {
                    var bytea = await rsp.Content.ReadAsByteArrayAsync();
                    string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                    var inp = ParseContent(ctyp, bytea, bytea.Length);
                    var arr = inp.ToArray<D>(proj);
                    return ((short) rsp.StatusCode, arr);
                }
                return ((short) rsp.StatusCode, default);
            }
            catch
            {
                return (0, default);
            }
        }

        public async Task<short> PostAsync(string uri, IContent content, string authstring = null)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, uri);
                if (authstring != null)
                {
                    req.Headers.TryAddWithoutValidation(AUTHORIZATION, authstring);
                }
                req.Content = (HttpContent) content;
                req.Headers.TryAddWithoutValidation(CONTENT_TYPE, content.CType);
                req.Headers.TryAddWithoutValidation(CONTENT_LENGTH, content.Count.ToString());
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return (short) rsp.StatusCode;
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (content is DynamicBuilder cnt)
                {
                    cnt.Clear();
                }
            }
        }

        public async Task<(short, S)> PostAsync<S>(string uri, IContent content, string authstring = null) where S : class, ISource
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Post, uri);
                if (authstring != null)
                {
                    req.Headers.TryAddWithoutValidation(AUTHORIZATION, authstring);
                }
                req.Content = (HttpContent) content;
                req.Headers.TryAddWithoutValidation(CONTENT_TYPE, content.CType);
                req.Headers.TryAddWithoutValidation(CONTENT_LENGTH, content.Count.ToString());

                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                string ctyp = rsp.Content.Headers.GetValue(CONTENT_TYPE);
                if (ctyp == null)
                {
                    return ((short) rsp.StatusCode, null);
                }
                else
                {
                    var bytes = await rsp.Content.ReadAsByteArrayAsync();
                    var src = ParseContent(ctyp, bytes, bytes.Length, typeof(S)) as S;
                    return ((short) rsp.StatusCode, src);
                }
            }
            catch
            {
                return (0, default);
            }
            finally
            {
                if (content is DynamicBuilder cnt)
                {
                    cnt.Clear();
                }
            }
        }
    }
}