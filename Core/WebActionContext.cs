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

        public string TokenStr { get; internal set; }

        public bool Cookied { get; internal set; }

        // two levels of variable keys
        Var var, var2;

        Var arg;

        internal void ChainVar(string value, WebFolder folder)
        {
            if (folder != null)
            {
                if (var.Value == null) var = new Var(value, folder);
                else if (var2.Value == null) var2 = new Var(value, folder);
            }
            else if (arg.Value == null)
            {
                arg = new Var(value, null);
            }
        }

        public Var Var => var;

        public Var Var2 => var2;

        public Var Arg => arg;

        //
        // REQUEST
        //

        public string Method => Request.Method;

        public bool GET => "GET".Equals(Request.Method);

        public bool POST => "POST".Equals(Request.Method);

        public string Uri => Features.Get<IHttpRequestFeature>().RawTarget;

        Form query;

        // request entity (ArraySegment<byte>, JObj, JArr, Form, XElem, null)
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

        public Field this[int index] => Query[index];

        public Field this[string name] => Query[name];

        //
        // HEADER
        //

        public string Header(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                return vs;
            }
            return null;
        }

        public int? HeaderInt(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                string str = vs;
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
                string str = vs;
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
            else if (ctyp.StartsWith("multipart/form-data"))
            {
                int beq = ctyp.IndexOf("boundary=", 19);
                string boundary = "--" + ctyp.Substring(beq + 9);
                FormUploadParse p = new FormUploadParse(boundary, bytebuf, count);
                entity = p.Parse();
                BufferUtility.Return(bytebuf); // return to the pool
            }
            else if (ctyp.StartsWith("application/json"))
            {
                JsonParse p = new JsonParse(bytebuf, count);
                entity = p.Parse();
                BufferUtility.Return(bytebuf); // return to the pool
            }
            else if (ctyp.StartsWith("application/xml"))
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

        public async Task<ArraySegment<byte>?> GetBytesSegAsync()
        {
            return (entity = await ReadAsync()) as ArraySegment<byte>?;
        }

        public async Task<Form> GetFormAsync()
        {
            return (entity = await ReadAsync()) as Form;
        }

        public async Task<JObj> GetJObjAsync()
        {
            return (entity = await ReadAsync()) as JObj;
        }

        public async Task<JArr> GetJArrAsync()
        {
            return (entity = await ReadAsync()) as JArr;
        }

        public async Task<D> GetDataObjAsync<D>(byte bits = 0) where D : IData, new()
        {
            ISource src = (entity = await ReadAsync()) as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToDataObj<D>(bits);
        }

        public async Task<D[]> GetDataObjsAsync<D>(byte bits = 0) where D : IData, new()
        {
            JArr jarr = (entity = await ReadAsync()) as JArr;
            return jarr?.ToDataArr<D>(bits);
        }

        public async Task<XElem> GetXElemAsync()
        {
            object entity = await ReadAsync();

            return entity as XElem;
        }

        //
        // RESPONSE
        //

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
        public int Seconds { get; internal set; }

        public void Reply(int status, IContent content, bool? pub = null, int seconds = 5)
        {
            Response.StatusCode = status;
            Content = content;
            Pub = pub;
            Seconds = seconds;
        }

        public void Reply(int status, bool? pub = null, int seconds = 5)
        {
            Reply(status, (IContent)null, pub, seconds);
        }

        public void Reply(int status, string str, bool? pub = null, int seconds = 30)
        {
            StrContent cont = new StrContent(true, true);
            cont.Add(str);

            Reply(status, cont, pub, seconds);
        }

        public void Reply(int status, IData data, byte bits = 0, bool? pub = null, int seconds = 30)
        {
            JsonContent cont = new JsonContent(true, true, 4 * 1024);
            cont.Put(null, data, bits);
            Reply(status, cont, pub, seconds);
        }

        public void Reply<D>(int status, D[] datas, byte bits = 0, bool? pub = null, int seconds = 30) where D : IData
        {
            JsonContent cont = new JsonContent(true, true, 4 * 1024);
            cont.Put(null, datas, bits);
            Reply(status, cont, pub, seconds);
        }

        internal async Task SendAsync()
        {
            SetHeader("Connection", "keep-alive");

            if (Pub != null)
            {
                string cc = Pub.Value ? "public" : "private" + ", max-age=" + Seconds * 1000;
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
                int sc = Response.StatusCode;
                if (GET && Pub == true)
                {
                    return sc == 200 || sc == 203 || sc == 204 || sc == 206 || sc == 300 || sc == 301 || sc == 404 ||
                           sc == 405 || sc == 410 || sc == 414 || sc == 501;
                }
                return false;
            }
        }

        public void Dispose()
        {
            // return request content buffer
            ArraySegment<byte>? bytesseg = entity as ArraySegment<byte>?;
            if (bytesseg != null)
            {
                BufferUtility.Return(bytesseg.Value.Array);
            }
        }
    }
}