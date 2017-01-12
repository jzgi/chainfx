using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The connect to a remote peer service that the current service depends on.
    ///
    public class WebClient : HttpClient, IRollable
    {
        const int
            INITIAL = -1,
            TIME_OUT = 60,
            TIME_OUT_2 = 120,
            NO_CONTENT = 12,
            NOT_IMPLEMENTED = 720;

        WebService service;

        // subdomain name or a reference name
        readonly string name;

        //
        // event polling & processing
        //

        bool status;

        // tick count
        int lastConnect;


        public WebClient(string name, string raddr)
        {
            this.name = name;
            string addr = raddr.StartsWith("http") ? raddr : "http://" + raddr;
            BaseAddress = new Uri(addr);
        }

        public string Name => name;

        internal async void PollAsync()
        {
            if (lastConnect < 100)
            {
                return;
            }

            HttpResponseMessage resp = await GetAsync("*");

            byte[] cont = await resp.Content.ReadAsByteArrayAsync();

            WebEventContext ec = new WebEventContext(this);
            FormMpParse p = new FormMpParse("", cont, cont.Length)
            {
                EventContext = ec
            };
            p.Parse(x =>
            {
                long id;
                string name = "";
                DateTime time;
                WebEvent handler = null;
                if (service.Events.TryGet(name, out handler))
                {
                    handler.Do(ec);
                }
            });
        }

        internal void SetCancel()
        {
            status = false;
        }

        public async Task<HttpResponseMessage> SendAsync(ICaller ctx, Action<HttpRequestMessage> areq)
        {
            HttpRequestMessage req = new HttpRequestMessage();
            areq(req);

            if (ctx.Cookied)
            {
                req.Headers.Add("Cookie", ctx.TokenStr);
            }
            else
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenStr);
            }
            return await SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public async Task<JArr> GetJArrAync(ICaller ctx, string uri)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Cookie", ctx.TokenStr);
            }
            else
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenStr);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.GetJArrAsync();
        }

        public async Task<D> GetDatAync<D>(ICaller ctx, string uri, byte flags = 0) where D : IDat, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Cookie", ctx.TokenStr);
            }
            else
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenStr);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.GetObjectAsync<D>(flags);
        }

        public async Task<D[]> GetDatsAync<D>(ICaller ctx, string uri, byte flags = 0) where D : IDat, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Cookie", ctx.TokenStr);
            }
            else
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenStr);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.GetArrayAsync<D>(flags);
        }

        public async Task<XElem> GetXElemAync(ICaller ctx, string uri)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Cookie", ctx.TokenStr);
            }
            else
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenStr);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.GetXElemAsync();
        }

        public async Task<byte[]> GetBytesAync(ICaller ctx, string uri)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Cookie", ctx.TokenStr);
            }
            else
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenStr);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.GetBytesSegAsync();
        }

        public Task<HttpResponseMessage> PostAsync<D>(ICaller ctx, string uri, D dat) where D : IDat
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Authorization", "Bearer " + "");
            }
            else
            {
                req.Headers.Add("Cookie", "");
            }
            JsonContent cont = new JsonContent(true, true);
            dat.Dump(cont);
            req.Content = cont;
            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public Task<HttpResponseMessage> PostAsync<D>(ICaller ctx, string uri, D[] dat) where D : IDat
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Authorization", "Bearer " + "");
            }
            else
            {
                req.Headers.Add("Cookie", "");
            }
            JsonContent cont = new JsonContent(true, true);
            req.Content = cont;
            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public Task<HttpResponseMessage> POST(ICaller ctx, string uri, byte[] dat)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx.Cookied)
            {
                req.Headers.Add("Authorization", "Bearer " + "");
            }
            else
            {
                req.Headers.Add("Cookie", "");
            }
            JsonContent cont = new JsonContent(true, true);
            req.Content = cont;
            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }
    }
}