using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Greatbone.Core.EventQueue;
using System.Data;
using static Greatbone.Core.DataInputUtility;

namespace Greatbone.Core
{
    ///
    /// A client of RPC, service and/or event queue.
    ///
    public class Connector : HttpClient, IRollable
    {
        static readonly Uri PollUri = new Uri("*", UriKind.Relative);

        readonly Service service;

        // prepared header value
        readonly string x_event;

        // target serviceid
        readonly string targetid;

        // this field is only accessed by the scheduler
        Task pollTask;

        // point of time to retry, set due to timeout or disconnection
        volatile int retryat;

        internal long evtid;

        public Connector(string raddr) : this(null, null, raddr) { }

        public Connector(Service service, string targetid, string raddr)
        {
            this.service = service;
            this.targetid = targetid;

            if (service != null) // build lastevent poll condition
            {
                Roll<EventInfo> eis = service.Events;
                if (eis != null)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < eis.Count; i++)
                    {
                        if (i > 0) sb.Append(',');
                        sb.Append(eis[i].Name);
                    }
                    x_event = sb.ToString();
                }
            }

            BaseAddress = new Uri(raddr);
            Timeout = TimeSpan.FromSeconds(5);
        }

        public string Name => targetid;


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
                    reqhs.TryAddWithoutValidation(X_EVENT, x_event);
                    reqhs.TryAddWithoutValidation(X_SHARD, service.Shard);

                    HttpResponseMessage rsp = null;
                    try
                    {
                        rsp = await SendAsync(req);
                        if (rsp.StatusCode == HttpStatusCode.NoContent)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        retryat = Environment.TickCount + 15000;
                        return;
                    }

                    HttpResponseHeaders rsphs = rsp.Headers;
                    byte[] cont = await rsp.Content.ReadAsByteArrayAsync();
                    EventContext ec = new EventContext(this)
                    {
                        id = rsphs.GetValue(X_ID).ToLong(),
                        // time = rsphs.GetValue(X_ARG)
                    };

                    // parse and process one by one
                    long id = 0;
                    string name = rsp.Headers.GetValue(X_EVENT);
                    string arg = rsp.Headers.GetValue(X_ARG);
                    DateTime time;
                    EventInfo ei = null;

                    using (var dc = ec.NewDbContext(IsolationLevel.ReadUncommitted))
                    {
                        if (service.Events.TryGet(name, out ei))
                        {
                            if (ei.IsAsync)
                            {
                                await ei.DoAsync(ec, arg);
                            }
                            else
                            {
                                ei.Do(ec, arg);
                            }
                        }

                        // database last id
                        dc.Execute("UPDATE evtu SET evtid = @1 WHERE peerid = @2", p => p.Set(id).Set(targetid));
                    }
                }
            });
        }

        //
        // RPC
        //

        public async Task<byte[]> GetAsync(ActionContext ctx, string uri)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("Authorization", "Bearer " + ctx.Token);
            HttpResponseMessage resp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            return await resp.Content.ReadAsByteArrayAsync();
        }

        public async Task<M> GetAsync<M>(ActionContext ac, string uri) where M : class, IDataInput
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ac != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ac.Token);
            }
            HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
            if (rsp.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            byte[] bytea = await rsp.Content.ReadAsByteArrayAsync();
            string ctyp = rsp.Content.Headers.GetValue("Content-Type");
            return (M)DataInputUtility.ParseContent(ctyp, bytea, 0, bytea.Length, typeof(M));
        }

        public async Task<D> GetObjectAsync<D>(ActionContext ctx, string uri, int proj = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.Token);
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
                req.Headers.Add("Authorization", "Bearer " + ctx.Token);
            }
            HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);

            IDataInput srcset = null;
            return srcset.ToArray<D>(proj);
        }

        public async Task<List<D>> GetListAsync<D>(ActionContext ctx, string uri, int proj = 0) where D : IData, new()
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.Token);
            }
            HttpResponseMessage rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);

            IDataInput srcset = null;
            return srcset.ToList<D>(proj);
        }

        public Task<HttpResponseMessage> PostAsync<C>(ActionContext ctx, string uri, C content) where C : HttpContent, IContent
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.Token);
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
                req.Headers.Add("Authorization", "Bearer " + ctx.Token);
            }
            IContent cont = inp.Dump();
            req.Content = (HttpContent)cont;
            req.Content.Headers.ContentType.MediaType = cont.Type;
            req.Content.Headers.ContentLength = cont.Size;

            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }


        public Task<HttpResponseMessage> PostJsonAsync(ActionContext ctx, string uri, object state)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri);
            if (ctx != null)
            {
                req.Headers.Add("Authorization", "Bearer " + ctx.Token);
            }

            if (state is Form)
            {

            }
            else if (state is JObj)
            {
                JsonContent cont = new JsonContent(true, true);
                ((JObj)state).WriteData(cont);
                req.Content = cont;
            }
            else if (state is IData)
            {
                JsonContent cont = new JsonContent(true, true);
                ((JObj)state).WriteData(cont);
                req.Content = cont;
            }
            return SendAsync(req, HttpCompletionOption.ResponseContentRead);
        }
    }
}