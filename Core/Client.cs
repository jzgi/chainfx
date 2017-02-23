using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The connect to a remote peer service that the current service depends on.
    ///
    public class Client : HttpClient, IRollable
    {
        const int
            INITIAL = -1,
            TIME_OUT = 60,
            TIME_OUT_2 = 120,
            NO_CONTENT = 12,
            NOT_IMPLEMENTED = 720;

        Service service;

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


        public Client(string name, string raddr)
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

            PollHandleAsync();
        }

        /// NOTE: We make it async void because the scheduler doesn't need to await this method
        internal async void PollHandleAsync()
        {
            HttpResponseMessage resp = await GetAsync("*");
            byte[] cont = await resp.Content.ReadAsByteArrayAsync();

            EventContext ec = new EventContext(this);

            // parse and process one by one
            long id;
            string name = "";
            string arg = "";
            DateTime time;
            EventInfo evt = null;
            if (service.Events.TryGet(name, out evt))
            {
                if (evt.IsAsync)
                {
                    await evt.DoAsync(ec, arg);
                }
                else
                {
                    evt.Do(ec, arg);
                }
            }
        }

        internal void SetCancel()
        {
            status = false;
        }

        //
        // RPC
        //

        public async Task<byte[]> GetAsync(ActionContext ctx, string uri)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.Content.ReadAsByteArrayAsync();
        }

        public async Task<M> GetAsync<M>(ActionContext ctx, string uri) where M : class, IDataInput
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

        public async Task<D> GetObjectAsync<D>(ActionContext ctx, string uri, int proj = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            IDataInput src = null;
            return src.ToObject<D>(proj);
        }

        public async Task<D[]> GetArrayAsync<D>(ActionContext ctx, string uri, int proj = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);

            IDataInput srcset = null;
            return srcset.ToArray<D>(proj);
        }

        public async Task<List<D>> GetListAsync<D>(ActionContext ctx, string uri, int proj = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);

            IDataInput srcset = null;
            return srcset.ToList<D>(proj);
        }

        public Task<HttpResponseMessage> PostAsync<C>(ActionContext ctx, string uri, C content) where C : HttpContent, IContent
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

        public Task<HttpResponseMessage> PostAsync(ActionContext ctx, string uri, IDataInput inp)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.TokenText);
            }
            IContent cont = inp.Dump();
            req.Content = (HttpContent)cont;
            req.Content.Headers.ContentType.MediaType = cont.Type;
            req.Content.Headers.ContentLength = cont.Size;

            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }


        public Task<HttpResponseMessage> PostJsonAsync(ActionContext ctx, string uri, object model)
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
                ((JObj)model).WriteData(cont);
                req.Content = cont;
            }
            else if (model is IData)
            {
                JsonContent cont = new JsonContent(true, true);
                ((JObj)model).WriteData(cont);
                req.Content = cont;
            }
            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }
    }
}