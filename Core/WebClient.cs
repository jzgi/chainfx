using System;
using System.Collections.Generic;
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

        bool connect;

        // last status
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

        public void ToPoll(int ticks)
        {
            if (lastConnect < 100)
            {
                return;
            }

            PollAsync();
        }

        /// NOTE: We make it async void because the scheduler doesn't need to await this method
        internal async void PollAsync()
        {

            HttpResponseMessage resp = await GetAsync("*");

            byte[] cont = await resp.Content.ReadAsByteArrayAsync();

            WebEventContext ec = new WebEventContext(this);
            FormMpParse p = new FormMpParse("", cont, cont.Length)
            {
                EventContext = ec
            };
            // parse and process one by one
            p.Parse(async x =>
            {
                long id;
                string name = "";
                DateTime time;
                WebEvent evt = null;
                if (service.Events.TryGet(name, out evt))
                {
                    if (evt.Async)
                    {
                        await evt.DoAsync(ec, null);
                    }
                    else
                    {
                        evt.Do(ec, null);
                    }
                }
            });
        }

        internal void SetCancel()
        {
            status = false;
        }

        //
        // RPC
        //

        public async Task<byte[]> GetAsync(WebActionContext ctx, string uri)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.Content.ReadAsByteArrayAsync();
        }

        public async Task<M> GetAsync<M>(WebActionContext ctx, string uri) where M : class, IModel
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            byte[] bytea = await resp.Content.ReadAsByteArrayAsync();
            string ctyp = resp.Content.Headers.GetValue("Content-Type");
            return (M)WebUtility.ParseContent(ctyp, bytea, 0, bytea.Length);
        }

        public async Task<D> GetObjectAsync<D>(WebActionContext ctx, string uri, byte flags = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            ISource src = null;
            return src.ToObject<D>(flags);
        }

        public async Task<D[]> GetArrayAsync<D>(WebActionContext ctx, string uri, byte flags = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);

            ISourceSet srcset = null;
            return srcset.ToArray<D>(flags);
        }

        public async Task<List<D>> GetListAsync<D>(WebActionContext ctx, string uri, byte flags = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);

            ISourceSet srcset = null;
            return srcset.ToList<D>(flags);
        }

        public Task<HttpResponseMessage> PostAsync<C>(WebActionContext ctx, string uri, C content) where C : HttpContent, IContent
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            req.Content = content;
            req.Headers.Add("Content-Type", content.Type);
            req.Headers.Add("Content-Length", content.Size.ToString());

            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }

        public Task<HttpResponseMessage> PostAsync(WebActionContext ctx, string uri, IModel model)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            IContent cont = model.Dump();
            req.Content = (HttpContent)cont;
            req.Content.Headers.ContentType.MediaType = cont.Type;
            req.Content.Headers.ContentLength = cont.Size;

            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }


        public Task<HttpResponseMessage> PostJsonAsync(WebActionContext ctx, string uri, object model)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }

            if (model is Form)
            {

            }
            else if (model is JObj)
            {
                JsonContent cont = new JsonContent(true, true);
                ((JObj)model).Dump(cont);
                req.Content = cont;
            }
            else if (model is IData)
            {
                JsonContent cont = new JsonContent(true, true);
                ((JObj)model).Dump(cont);
                req.Content = cont;
            }
            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }
    }
}