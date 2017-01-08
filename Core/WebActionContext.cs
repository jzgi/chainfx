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
    public class WebActionContext : DefaultHttpContext, ICaller, IDisposable
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
        Var key, key2;

        Var arg;

        internal void ChainKey(string value, WebFolder folder)
        {
            if (key.Empty)
            {
                key = new Var(value, folder);
            }
            else if (key2.Empty)
            {
                key2 = new Var(value, folder);
            }
        }

        public Var Key => key;

        public Var Key2 => key2;

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

        public long? HeaderLong(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                string str = vs;
                long v;
                if (long.TryParse(str, out v))
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
        async Task<object> ReadAsync(int len)
        {
            byte[] buf = BufferUtility.ByteBuffer(len); // borrow from the pool
            int count = len;
            int offset = 0;
            int num;
            while ((num = await Request.Body.ReadAsync(buf, offset, count)) < count)
            {
                offset += num;
                count -= num;
            }

            string ctyp = Request.ContentType;
            if ("application/x-www-form-urlencoded".Equals(ctyp))
            {
                entity = new FormParse(buf, len).Parse();
                BufferUtility.Return(buf); // return to the pool
            }
            else if (ctyp.StartsWith("multipart/form-data"))
            {
                int bdy = ctyp.IndexOf("boundary=", 19, StringComparison.Ordinal);
                string boundary = ctyp.Substring(bdy + 9);
                entity = new FormMpParse(boundary, buf, len).Parse();
                // NOTE: the form's backing buffer shall reutrn pool during Dispose()
            }
            else if (ctyp.StartsWith("application/json"))
            {
                entity = new JsonParse(buf, len).Parse();
                BufferUtility.Return(buf); // return to the pool
            }
            else if (ctyp.StartsWith("application/xml"))
            {
                entity = new XmlParse(buf, len).Parse();
                BufferUtility.Return(buf); // return to the pool
            }
            else
            {
                entity = new ArraySegment<byte>(buf, 0, len);
            }
            return entity;
        }

        public async Task<ArraySegment<byte>?> AsBytesSegAsync()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            return entity as ArraySegment<byte>?;
        }

        public async Task<ISource> AsSourceAsync()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            return entity as ISource;
        }

        public async Task<Form> AsFormAsync()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            return entity as Form;
        }

        public async Task<JObj> AsJObjAsync()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            return entity as JObj;
        }

        public async Task<JArr> AsJArrAsync()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            return entity as JArr;
        }

        public async Task<D> AsObjectAsync<D>(byte flags = 0) where D : IData, new()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            ISource src = entity as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(flags);
        }

        public async Task<D[]> AsArrayAsync<D>(byte flags = 0) where D : IData, new()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            JArr jarr = entity as JArr;
            return jarr?.ToArray<D>(flags);
        }

        public async Task<XElem> AsXElemAsync()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    entity = await ReadAsync((int)clen);
                }
            }
            return entity as XElem;
        }

        //
        // RESPONSE
        //

        public void Header(string name, int v)
        {
            Response.Headers.Add(name, new StringValues(v.ToString()));
        }

        public void Header(string name, string v)
        {
            Response.Headers.Add(name, new StringValues(v));
        }

        public void Header(string name, DateTime v)
        {
            string str = StrUtility.FormatUtcDate(v);
            Response.Headers.Add(name, new StringValues(str));
        }

        public void Header(string name, params string[] values)
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

        public void Reply(int status, IData obj, byte flags = 0, bool? pub = null, int seconds = 30)
        {
            JsonContent cont = new JsonContent(true, true, 4 * 1024);
            cont.Put(null, obj, flags);
            Reply(status, cont, pub, seconds);
        }

        public void Reply<D>(int status, D[] arr, byte flags = 0, bool? pub = null, int seconds = 30) where D : IData
        {
            JsonContent cont = new JsonContent(true, true, 4 * 1024);
            cont.Put(null, arr, flags);
            Reply(status, cont, pub, seconds);
        }

        public void Reply(int status, Form form, bool? pub = null, int seconds = 30)
        {
        }

        public void Reply(int status, JObj obj, bool? pub = null, int seconds = 30)
        {
        }

        public void Reply(int status, JArr arr, bool? pub = null, int seconds = 30)
        {
        }

        public void Reply(int status, XElem elem, bool? pub = null, int seconds = 30)
        {
        }

        public void Reply(int status, ArraySegment<byte> bytesseg, bool? pub = null, int seconds = 30)
        {
        }

        internal async Task SendAsync()
        {
            Header("Connection", "keep-alive");

            if (Pub != null)
            {
                string cc = Pub.Value ? "public" : "private" + ", max-age=" + Seconds * 1000;
                Header("Cache-Control", cc);
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
                    Header("ETag", StrUtility.ToHex(etag));
                }

                // set last-modified
                DateTime? last = Content.Modified;
                if (last != null)
                {
                    Header("Last-Modified", StrUtility.FormatUtcDate(last.Value));
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
            else
            {
                (entity as IReturnable)?.Return();
            }
        }
    }
}