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
    public class Client : HttpClient, IKeyable<string>
    {
        const int AHEAD = 1000 * 12;

        // remote service key 
        readonly string rkey;

        // this field is only accessed by the scheduler
        Task pollTask;

        long last;

        // point of time to retry, set due to timeout or disconnection
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
        /// Used to construct a service client that connects to a remote service. 
        /// </summary>
        /// <param name="rkey">remote service key</param>
        /// <param name="raddr">remote address</param>
        internal Client(string rkey, string raddr)
        {
            this.rkey = rkey;
            BaseAddress = new Uri(raddr);
            Timeout = TimeSpan.FromSeconds(5);
        }

        public string Key => rkey;


        internal void TryPoll(Action<WebContext> consumer, int ticks)
        {
            if (ticks < retryPt)
            {
                return;
            }
            if (pollTask != null && !pollTask.IsCompleted)
            {
                return;
            }
            // initialize lastid by the consumer itself
            if (last == 0)
            {
//                last = consumer(null);
            }

            pollTask = Task.Run(async () =>
            {
                for (;;)
                {
                    var uri = new Uri("*" + rkey + "?last=" + last, UriKind.Relative);
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                    try
                    {
                        var rsp = await SendAsync(req);
                        if (rsp.StatusCode == HttpStatusCode.NoContent)
                        {
                            break;
                        }
                        byte[] cont = await rsp.Content.ReadAsByteArrayAsync();
//                        consumer(new WebContext(cont, cont.Length));
                    }
                    catch
                    {
                        retryPt = Environment.TickCount + AHEAD;
                        return;
                    }
                }
            });
        }

        //
        // RPC
        //

        public async Task<byte[]> GetAsync(WebContext ac, string uri)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (rkey != null && ac != null)
                {
                    if (ac.Token != null)
                    {
                        req.Headers.Add("Authorization", "Token " + ac.Token);
                    }
                }

                HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return await resp.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                retryPt = Environment.TickCount + AHEAD;
            }
            return null;
        }

        public async Task<M> GetAsync<M>(string uri, string token) where M : class, ISource
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (rkey != null && token != null)
                {
                    req.Headers.Add("Authorization", "Token " + token);
                }
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
                retryPt = Environment.TickCount + AHEAD;
            }
            return null;
        }

        public async Task<D> GetObjectAsync<D>(string uri, string token, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (rkey != null && token != null)
                {
                    req.Headers.Add("Authorization", "Token " + token);
                }
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
                retryPt = Environment.TickCount + AHEAD;
            }
            return default;
        }

        public async Task<D[]> GetArrayAsync<D>(string uri, string token, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (rkey != null && token != null)
                {
                    req.Headers.Add("Authorization", "Token " + token);
                }
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
                retryPt = Environment.TickCount + AHEAD;
            }
            return null;
        }

        public async Task<int> PostAsync(string uri, IContent content, string token)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
                if (rkey != null && token != null)
                {
                    req.Headers.Add("Authorization", "Token " + token);
                }
                req.Content = (HttpContent) content;
                req.Headers.TryAddWithoutValidation("Content-Type", content.Type);
                req.Headers.TryAddWithoutValidation("Content-Length", content.Size.ToString());

                HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                return (int) rsp.StatusCode;
            }
            catch
            {
                retryPt = Environment.TickCount + AHEAD;
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
                retryPt = Environment.TickCount + AHEAD;
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