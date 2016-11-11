using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    ///
    /// The encapsulation of a web request/response exchange context.
    ///
    public class WebContext : DefaultHttpContext, IDisposable
    {
        internal WebContext(IFeatureCollection features) : base(features)
        {
        }

        public WebDir Dir { get; internal set; }

        public WebAction Action { get; internal set; }

        public IPrincipal Principal { get; internal set; }


        // the chain of keys
        Var[] chain;
        int vars;

        internal void Chain(WebDir dir, string key)
        {
            if (chain == null)
            {
                chain = new Var[4];
            }
            chain[vars++] = new Var(dir, key);
        }

        public Var Var<V>(V dir) where V : WebDir, IVariable
        {
            for (int i = vars - 1; i >= 0; i--)
            {
                if (chain[i].Dir == dir) return chain[i];
            }
            return default(Var);
        }

        //
        // REQUEST
        //
        public string Method => Request.Method;

        public bool IsGetMethod => "GET".Equals(Request.Method);

        public bool IsPostMethod => "POST".Equals(Request.Method);

        Form query;

        // received request body
        byte[] bytebuf;
        int count; // number of received bytes

        // parsed request entity (JObj, JArr, Form, null)
        object entity;

        public Form Query
        {
            get
            {
                if (query == null)
                {
                    string qstr = Request.QueryString.Value;
                    FormParse p = new FormParse(qstr);
                    query = p.Parse(); // non-null
                }
                return query;
            }
        }

        public Pair this[int index] => Query[index];

        public Pair this[string name] => Query[name];

        //
        // HEADER
        //

        public string Header(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                return (string)vs;
            }
            return null;
        }

        public int? HeaderInt(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                string str = (string)vs;
                int v;
                if (int.TryParse(str, out v))
                {
                    return v;
                }
            }
            return null;
        }

        public DateTime? HeaderDateTime(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                string str = (string)vs;
                DateTime v;
                if (StrUtility.TryParseUtcDate(str, out v))
                {
                    return v;
                }
            }
            return null;
        }

        async void EnsureReadAsync()
        {
            if (count > 0) return;

            HttpRequest req = Request;
            long? clen = req.ContentLength;
            if (clen > 0)
            {
                int len = (int)clen;
                bytebuf = BufferUtility.GetByteBuffer(len);
                count = await req.Body.ReadAsync(bytebuf, 0, len);
            }
        }

        public bool IsPooled => bytebuf != null;

        void EnsureParse()
        {
            if (entity != null) return;

            EnsureReadAsync();

            if (count > 0)
            {
                string ctyp = Request.ContentType;
                if ("application/x-www-form-urlencoded".Equals(ctyp))
                {
                    FormParse p = new FormParse(bytebuf, count);
                    entity = p.Parse();
                }
                else if ("application/xml".Equals(ctyp))
                {
                    XmlParse p = new XmlParse(bytebuf, count);
                    entity = p.Parse();
                }
                else
                {
                    bool jx = "application/jsonx".Equals(ctyp); // json extention
                    JsonParse p = new JsonParse(bytebuf, count, jx);
                    entity = p.Parse();
                }
            }
        }

        public ArraySegment<byte>? ReadByteA()
        {
            EnsureReadAsync();

            if (count == 0) return null;

            return new ArraySegment<byte>(bytebuf, 0, count);
        }

        public Form ReadForm()
        {
            EnsureParse();

            return entity as Form;
        }

        public Obj ReadObj()
        {
            EnsureParse();

            return entity as Obj;
        }

        public Arr ReadArr()
        {
            EnsureParse();

            return entity as Arr;
        }

        public D ReadData<D>(byte z = 0) where D : IData, new()
        {
            EnsureParse();

            ISource src = entity as ISource;
            if (src == null) return default(D);
            D dat = new D();
            dat.Load(src, z);
            return dat;
        }

        public D[] ReadDatas<D>(byte z = 0) where D : IData, new()
        {
            EnsureParse();

            Arr arr = entity as Arr;
            return arr?.ToDatas<D>(z);
        }

        public Elem ReadElem()
        {
            EnsureParse();

            return entity as Elem;
        }

        //
        // RESPONSE
        //

        public int StatusCode
        {
            get { return Response.StatusCode; }
            set { Response.StatusCode = value; }
        }

        public void SetHeader(string name, int v)
        {
            Response.Headers.Add(name, new StringValues(v.ToString()));
        }

        public void SetHeader(string name, string v)
        {
            Response.Headers.Add(name, new StringValues(v));
        }

        public void SetHeader(string name, DateTime v)
        {
            string str = StrUtility.FormatUtcDate(v);
            Response.Headers.Add(name, new StringValues(str));
        }

        public void SetHeader(string name, params string[] values)
        {
            Response.Headers.Add(name, new StringValues(values));
        }

        public IContent Content { get; internal set; }

        // public, no-cache or private
        public bool? Pub { get; internal set; }

        // the content  is to be considered stale after its age is greater than the specified number of seconds.
        public int MaxAge { get; internal set; }

        public void Send(int status, IContent cont, bool? pub = null, int maxage = 60000)
        {
            StatusCode = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void SendText(int status, string text, bool? pub = null, int maxage = 60000)
        {
            StatusCode = status;
            Content = new PlainContent(true, true)
            {
                Text = text
            };

            Pub = pub;
            MaxAge = maxage;
        }

        public void SendJson<D>(int status, D dat, byte z = 0, bool? pub = null, int maxage = 60000) where D : IData
        {
            SendJson(status, cont => cont.PutObj(dat, z), pub, maxage);
        }

        public void SendJson<D>(int status, D[] dats, byte z = 0, bool? pub = null, int maxage = 60000) where D : IData
        {
            SendJson(status, cont => cont.PutArr(dats, z), pub, maxage);
        }

        public void SendJson(int status, Action<JsonContent> a, bool? pub = null, int maxage = 60000)
        {
            JsonContent cont = new JsonContent(true, true, 4 * 1024);
            a?.Invoke(cont);
            Send(status, cont, pub, maxage);
        }

        internal async Task SendAsync()
        {
            SetHeader("Connection", "keep-alive");

            if (Pub != null)
            {
                string cc = Pub.Value ? "public" : "private" + ", max-age=" + MaxAge;
                SetHeader("Cache-Control", cc);
            }

            // setup appropriate headers
            if (Content != null)
            {
                HttpResponse r = Response;
                r.ContentLength = Content.Size;
                r.ContentType = Content.Type;

                // cache indicators
                if (Content is DynamicContent) // set etag
                {
                    ulong etag = ((DynamicContent)Content).ETag;
                    SetHeader("ETag", StrUtility.ToHex(etag));
                }

                // set last-modified
                DateTime? last = Content.Modified;
                if (last != null)
                {
                    SetHeader("Last-Modified", StrUtility.FormatUtcDate(last.Value));
                }

                // send async
                await r.Body.WriteAsync(Content.ByteBuffer, 0, Content.Size);
            }
        }

        //
        // RPC
        //

        public async void CallByGet(string service, string part, string uri)
        {
            // token impersonate
            WebClient cli = Dir.Service.FindClient(service, part);
            if (cli != null)
            {
                object obj = await cli.GetAsync(uri);
            }
        }

        public void CallByPost(string service, string part, Action<JsonContent> a)
        {
            // token impersonate
            WebClient cli = Dir.Service.FindClient(service, part);
            if (cli != null)
            {
                JsonContent cont = new JsonContent(true, true, 8 * 1024);
                a?.Invoke(cont);
                BufferUtility.Return(cont);
            }
        }


        public void Dispose()
        {
            // return request content buffer
            if (IsPooled)
            {
                BufferUtility.Return(bytebuf);
            }

            // return response content buffer
            if (Content != null && Content.IsPooled)
            {
                BufferUtility.Return(Content.ByteBuffer);
            }
        }
    }
}