using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        static readonly Uri PollUri = new Uri("*", UriKind.Relative);

        readonly Service service;

        // remote peer id 
        readonly string peerId;

        // this field is only accessed by the scheduler
        Task pollTask;

        // point of time to retry, set due to timeout or disconnection
        volatile int retryat;

        public string PeerId => peerId;

        public Client(HttpClientHandler handler) : base(handler)
        {
        }

        public Client(string raddr) : this(null, null, raddr)
        {
        }

        internal Client(Service service, string peerId, string raddr)
        {
            this.service = service;
            this.peerId = peerId;


            BaseAddress = new Uri(raddr);
            Timeout = TimeSpan.FromSeconds(5);
        }

        public string Key => peerId;


        public void TryPoll(int ticks)
        {
            if (ticks < retryat)
            {
                return;
            }

            if (pollTask != null && !pollTask.IsCompleted)
            {
                return;
            }

            pollTask = Task.Run(async () =>
            {
                for (;;)
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, PollUri);
                    HttpRequestHeaders reqhs = req.Headers;
                    reqhs.TryAddWithoutValidation("From", service.Id);

                    HttpResponseMessage rsp;
                    try
                    {
                        rsp = await SendAsync(req);
                        if (rsp.StatusCode == HttpStatusCode.NoContent)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        retryat = Environment.TickCount + AHEAD;
                        return;
                    }

                    HttpResponseHeaders rsphs = rsp.Headers;
                    byte[] cont = await rsp.Content.ReadAsByteArrayAsync();
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
                if (peerId != null && ac != null)
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
                retryat = Environment.TickCount + AHEAD;
            }

            return null;
        }

        public async Task<M> GetAsync<M>(string uri, string token) where M : class, ISource
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (peerId != null && token != null)
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
                retryat = Environment.TickCount + AHEAD;
            }
            return null;
        }

        public async Task<D> GetObjectAsync<D>(string uri, string token, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (peerId != null && token != null)
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
                retryat = Environment.TickCount + AHEAD;
            }
            return default;
        }

        public async Task<D[]> GetArrayAsync<D>(string uri, string token, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
                if (peerId != null && token != null)
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
                retryat = Environment.TickCount + AHEAD;
            }
            return null;
        }

        public async Task<int> PostAsync(string uri, IContent content, string token)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
                if (peerId != null && token != null)
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
                retryat = Environment.TickCount + AHEAD;
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
                retryat = Environment.TickCount + AHEAD;
            }
            finally
            {
                if (content is DynamicContent cont)
                {
                    BufferUtility.Return(cont);
                }
            }
            return default((int, M));
        }
    }
}