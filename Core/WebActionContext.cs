using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
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

        internal void ChainKey(string key, WebFolder folder)
        {
            if (this.key.Empty)
            {
                this.key = new Var(key, folder);
            }
            else if (key2.Empty)
            {
                key2 = new Var(key, folder);
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

        int count = -1;

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
                if (TextUtility.TryParseUtcDate(str, out v))
                {
                    return v;
                }
            }
            return null;
        }

        public IRequestCookieCollection Cookies => Request.Cookies;

        public async Task<ArraySegment<byte>> ReadAsync()
        {
            if (count == -1) // if not yet read
            {
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    // reading
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
            }
            return new ArraySegment<byte>(buffer, 0, count);
        }

        bool TryParse()
        {
            // parse into entity if content type is known
            string ctyp = Header("Content-Type");

            if (string.IsNullOrEmpty(ctyp)) return false;

            if ("application/x-www-form-urlencoded".Equals(ctyp))
            {
                entity = new FormParse(buffer, count).Parse();
                return true;
            }
            if (ctyp.StartsWith("multipart/form-data; boundary="))
            {
                string boundary = ctyp.Substring(30);
                entity = new FormMpParse(boundary, buffer, count).Parse();
                return true;
            }
            if (ctyp.StartsWith("application/json"))
            {
                entity = new JsonParse(buffer, count).Parse();
                return true;
            }
            if (ctyp.StartsWith("application/xml"))
            {
                entity = new XmlParse(buffer, 0, count).Parse();
                return true;
            }
            if (ctyp.StartsWith("text/plain"))
            {
                Text txt = new Text();
                byte[] buffer = this.buffer;
                for (int i = 0; i < count; i++)
                {
                    txt.Accept(buffer[i]);
                }
                entity = txt;
                return true;
            }
            return false;
        }

        public async Task<M> ReadAsync<M>() where M : class, IContentModel
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    // reading
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                TryParse();
            }
            return entity as M;
        }

        public async Task<D> ReadUnAsync<D>(byte flags = 0) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    // reading
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                // parse
                TryParse();
            }
            ISource src = entity as ISource;
            if (src == null)
            {
                return default(D);
            }
            return src.ToUn<D>(flags);
        }

        public async Task<D[]> ReadArrayAsync<D>(byte flags = 0) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    // reading
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                TryParse();
            }
            return (entity as ISourceSet)?.ToArray<D>(flags);
        }

        public async Task<List<D>> ReadListAsync<D>(byte flags = 0) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    // reading
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                TryParse();
            }
            return (entity as ISourceSet)?.ToList<D>(flags);
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
            string str = TextUtility.FormatUtcDate(v);
            Response.Headers.Add(name, new StringValues(str));
        }

        public void SetHeader(string name, params string[] values)
        {
            Response.Headers.Add(name, new StringValues(values));
        }

        public int Status
        {
            get { return Response.StatusCode; }
            set { Response.StatusCode = value; }
        }

        public IContent Content { get; internal set; }

        // public, no-cache or private
        public bool? Pub { get; internal set; }

        /// the cached response is to be considered stale after its age is greater than the specified number of seconds.
        public int MaxAge { get; internal set; }

        public void Reply(int status, IContent cont = null, bool? pub = null, int maxage = 60)
        {
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Reply(int status, IContentModel model, bool? pub = null, int maxage = 60)
        {
            Response.StatusCode = status;
            // Content = content;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Reply(int status, string str, bool? pub = null, int maxage = 60)
        {
            TextContent cont = new TextContent(true, false, 256);
            cont.Add(str);

            // set response states
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void ReplyJson(int status, object dat, byte flags = 0, bool? pub = null, int maxage = 60)
        {
            TypeInfo typ = dat.GetType().GetTypeInfo();

            JsonContent cont = new JsonContent(true, true, 4 * 1024);

            if (typeof(IData).GetTypeInfo().IsAssignableFrom(typ))
            {
                cont.Put(null, (IData)dat, flags);
            }
            else if (typeof(IData[]).GetTypeInfo().IsAssignableFrom(typ))
            {
                cont.Put(null, (IData[])dat, flags);

            }
            else if (typeof(List<IData>).GetTypeInfo().IsAssignableFrom(typ))
            {
                cont.Put(null, (List<IData>)dat, flags);
            }

            // set response states
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void ReplyXml(int status, object dat, byte flags = 0, bool? pub = null, int maxage = 60)
        {
        }


        internal async Task SendAsync()
        {
            // set connection header if absent
            SetHeaderNon("Connection", "keep-alive");

            if (Pub != null)
            {
                if (Pub.HasValue)
                {
                    string hv = (Pub.Value ? "public" : "private") + ", max-age=" + MaxAge;
                    SetHeader("Cache-Control", hv);
                }
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
                    SetHeader("ETag", TextUtility.ToHex(etag));
                }

                // set last-modified
                DateTime? last = Content.Modified;
                if (last != null)
                {
                    SetHeader("Last-Modified", TextUtility.FormatUtcDate(last.Value));
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