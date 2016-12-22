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
    public class WebActionContext : DefaultHttpContext, ICallerContext, IDisposable
    {
        internal WebActionContext(IFeatureCollection features) : base(features)
        {
        }

        public WebFolder Folder { get; internal set; }

        public WebAction Action { get; internal set; }

        public IToken Token { get; internal set; }

        public string TokenString { get; internal set; }

        public bool IsCookied { get; internal set; }

        // two levels of variable keys
        Var x, x2;

        Var sub;

        internal void ChainVar(string value, WebFolder folder)
        {
            if (folder != null)
            {
                if (x.Key == null) x = new Var(value, folder);
                else if (x2.Key == null) x2 = new Var(value, folder);
            }
            else if (sub.Key == null)
            {
                sub = new Var(value, null);
            }
        }

        public Var X => x;

        public Var X2 => x2;

        public Var Sub => sub;

        //
        // REQUEST
        //

        public string Method => Request.Method;

        public bool GET => "GET".Equals(Request.Method);

        public bool POST => "POST".Equals(Request.Method);

        public string Uri => Features.Get<IHttpRequestFeature>().RawTarget;

        Form query;

        // request entity (ArraySegment<byte>, Obj, Arr, Form, Elem, null)
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

        public IRequestCookieCollection Cookies => Request.Cookies;

        // read and parse
        async Task<object> ReadAsync()
        {
            if (entity != null) return null;

            long? clen = Request.ContentLength;
            if (clen <= 0) return null;

            int len = (int)clen;
            byte[] bytebuf = BufferUtility.BorrowByteBuf(len); // borrow from the pool
            int count = await Request.Body.ReadAsync(bytebuf, 0, len);

            string ctyp = Request.ContentType;
            if ("application/x-www-form-urlencoded".Equals(ctyp))
            {
                FormParse p = new FormParse(bytebuf, count);
                entity = p.Parse();
                BufferUtility.Return(bytebuf); // return to the pool
            }
            else if ("application/json".Equals(ctyp))
            {
                JsonParse p = new JsonParse(bytebuf, count);
                entity = p.Parse();
                BufferUtility.Return(bytebuf); // return to the pool
            }
            else if ("application/xml".Equals(ctyp))
            {
                XmlParse p = new XmlParse(bytebuf, count);
                entity = p.Parse();
                BufferUtility.Return(bytebuf); // return to the pool
            }
            else
            {
                entity = new ArraySegment<byte>(bytebuf, 0, count);
            }
            return entity;
        }

        public async Task<ArraySegment<byte>?> GetArraySegAsync()
        {
            return (entity = await ReadAsync()) as ArraySegment<byte>?;
        }
        public async Task<Form> GetFormAsync()
        {
            return (entity = await ReadAsync()) as Form;
        }

        public async Task<Obj> GetObjAsync()
        {
            return (entity = await ReadAsync()) as Obj;
        }

        public async Task<Arr> GetArrAsync()
        {
            return (entity = await ReadAsync()) as Arr;
        }

        public async Task<D> GetDataAsync<D>(byte z = 0) where D : IData, new()
        {
            ISource src = (entity = await ReadAsync()) as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToData<D>(z);
        }

        public async Task<D[]> GetDatasAsync<D>(byte z = 0) where D : IData, new()
        {
            Arr arr = (entity = await ReadAsync()) as Arr;
            return arr?.ToDatas<D>(z);
        }

        public async Task<Elem> GetElemAsync()
        {
            object entity = await ReadAsync();

            return entity as Elem;
        }

        //
        // RESPONSE
        //

        public int Status
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

        public void Set(int status, IContent cont, bool? pub = null, int maxage = 60000)
        {
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void SetText(int status, string text, bool? pub = null, int maxage = 60000)
        {
            Status = status;
            StrContent cont = new StrContent(true, true);
            cont.Add(text);
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void SetJson<D>(int status, D dat, byte z = 0, bool? pub = null, int maxage = 60000) where D : IData
        {
            SetJson(status, cont => cont.Put(null, dat, z), pub, maxage);
        }

        public void SetJson<D>(int status, D[] dats, byte z = 0, bool? pub = null, int maxage = 60000) where D : IData
        {
            SetJson(status, cont => cont.Put(null, dats, z), pub, maxage);
        }

        public void SetJson(int status, Action<JsonContent> a, bool? pub = null, int maxage = 60000)
        {
            JsonContent cont = new JsonContent(true, true, 4 * 1024);
            a?.Invoke(cont);
            Set(status, cont, pub, maxage);
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
                r.ContentType = Content.MimeType;

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


        internal bool Cacheable
        {
            get
            {
                int sc = Status;
                if (GET && Pub == true)
                {
                    return sc == 200 || sc == 203 || sc == 204 || sc == 206 || sc == 300 || sc == 301 || sc == 404 || sc == 405 || sc == 410 || sc == 414 || sc == 501;
                }
                return false;
            }
        }

        public void Dispose()
        {
            // return request content buffer
            ArraySegment<byte>? arrayseg = entity as ArraySegment<byte>?;
            if (arrayseg != null)
            {
                BufferUtility.Return(arrayseg.Value.Array);
            }
        }
    }
}