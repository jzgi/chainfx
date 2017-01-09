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

        //
        // REQUEST
        //

        public string Method => Request.Method;

        public bool GET => "GET".Equals(Request.Method);

        public bool POST => "POST".Equals(Request.Method);

        public string Uri => Features.Get<IHttpRequestFeature>().RawTarget;

        // URL query 
        Form query;

        // request body
        byte[] buffer;

        int count;

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

        static readonly WebException ReadEx = new WebException("error reading request body");

        // read and parse
        async void ReadAsync(int len)
        {
            try
            {
                buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                {
                }

                string ctyp = Request.ContentType;
                object enty;
                if ("application/x-www-form-urlencoded".Equals(ctyp))
                {
                    enty = new FormParse(buffer, len).Parse();
                }
                else if (ctyp.StartsWith("multipart/form-data; boundary="))
                {
                    string boundary = ctyp.Substring(30);
                    enty = new FormMpParse(boundary, buffer, len).Parse();
                }
                else if (ctyp.StartsWith("application/json"))
                {
                    enty = new JsonParse(buffer, len).Parse();
                }
                else if (ctyp.StartsWith("application/xml"))
                {
                    enty = new XmlParse(buffer, len).Parse();
                }
                else
                {
                    enty = new ArraySegment<byte>(buffer, 0, len);
                }
                entity = enty;
            }
            catch (Exception e) // handle exception within this async method
            {
            }
        }

        static readonly ParseException ParseEx = new ParseException("error parsing request body");

        public ArraySegment<byte>? ReadBytesSeg()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
            return entity as ArraySegment<byte>?;
        }

        public ISource ReadSource()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
            return entity as ISource;
        }

        public Form ReadForm()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
            return entity as Form;
        }

        public JObj ReadJObj()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
            return entity as JObj;
        }

        public JArr ReadJArr()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
            return entity as JArr;
        }

        public D ReadObject<D>(byte flags = 0) where D : IData, new()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
            ISource src = entity as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(flags);
        }

        public D[] ReadArray<D>(byte flags = 0) where D : IData, new()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
            JArr jarr = entity as JArr;
            return jarr?.ToArray<D>(flags);
        }

        public XElem ReadXElem()
        {
            if (entity == null)
            {
                long? clen = Request.ContentLength;
                if (clen > 0)
                {
                    int len = (int) clen;
                    ReadAsync(len);
                    if (count != len) throw ReadEx;
                    if (entity == null) throw ParseEx;
                }
            }
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

        public void SetHeaderNon(string name, string v)
        {
            StringValues strvs;
            IHeaderDictionary headers = Response.Headers;
            if (!headers.TryGetValue(name, out strvs))
            {
                headers.Add(name, new StringValues(v));
            }
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
            Reply(status, (IContent) null, pub, seconds);
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
            // set connection header if absent
            SetHeaderNon("Connection", "keep-alive");

            if (Pub != null)
            {
                string cachectrl = Pub.Value ? "public" : "private" + ", max-age=" + Seconds * 1000;
                SetHeader("Cache-Control", cachectrl);
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
                    ulong etag = ((DynamicContent) Content).ETag;
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
            // request content buffer
            if (buffer != null)
            {
                BufferUtility.Return(buffer);
            }

            // response content buffer
            IContent cont = Content;
            if (cont != null && cont.Poolable)
            {
                BufferUtility.Return(cont.ByteBuffer);
            }
        }
    }
}