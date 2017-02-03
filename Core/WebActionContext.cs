using System;
using System.Collections.Generic;
using System.Data;
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
    public class WebActionContext : DefaultHttpContext, IHandleContext<WebAction>, IDisposable
    {
        internal WebActionContext(IFeatureCollection features) : base(features)
        {
        }

        public WebServiceContext ServiceContext { get; set; }

        public WebFolder Folder { get; internal set; }

        public WebAction Handle { get; internal set; }

        public IData Token { get; internal set; }

        public string TokenText { get; internal set; }

        // levels of variable keys
        Var[] vars;

        int varlen;

        internal void ChainVar(string key, WebFolder folder)
        {
            if (vars == null)
            {
                vars = new Var[4];
            }
            vars[varlen++] = new Var(key, folder);
        }

        public Var this[int level] => vars[level];

        public Var this[Type folderType]
        {
            get
            {
                for (int i = 0; i < varlen; i++)
                {
                    Var v = vars[i];
                    if (v.Type == folderType) return v;
                }
                return default(Var);
            }
        }

        public Var this[IVar folder]
        {
            get
            {
                for (int i = 0; i < varlen; i++)
                {
                    Var v = vars[i];
                    if (v.Folder == folder) return v;
                }
                return default(Var);
            }
        }

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

        public async Task<M> ReadAsync<M>() where M : class, IDataInput
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = WebUtility.ParseContent(ctyp, buffer, 0, count);
            }
            return entity as M;
        }

        public async Task<D> ReadObjectAsync<D>(ushort proj = 0) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = WebUtility.ParseContent(ctyp, buffer, 0, count);
            }
            IDataInput src = entity as IDataInput;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(proj);
        }

        public async Task<D[]> ReadArrayAsync<D>(ushort proj = 0) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = WebUtility.ParseContent(ctyp, buffer, 0, count);
            }
            return (entity as IDataInput)?.ToArray<D>(proj);
        }

        public async Task<List<D>> ReadListAsync<D>(ushort proj = 0) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    int len = (int)clen;
                    buffer = BufferUtility.ByteBuffer(len); // borrow from the pool
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = WebUtility.ParseContent(ctyp, buffer, 0, count);
            }
            return (entity as IDataInput)?.ToList<D>(proj);
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

        public void Reply(int status, IContent content = null, bool? pub = null, int maxage = 60)
        {
            Status = status;
            Content = content;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Reply(int status, IDataInput inp, bool? pub = null, int maxage = 60)
        {
            Status = status;
            // Content = model.Dump();
            Pub = pub;
            MaxAge = maxage;
        }

        public void Reply(int status, string text, bool? pub = null, int maxage = 60)
        {
            TextContent content = new TextContent(true);
            content.Add(text);

            // set response states
            Status = status;
            Content = content;
            Pub = pub;
            MaxAge = maxage;
        }

        static readonly TypeInfo ObjectType = typeof(IData).GetTypeInfo();

        static readonly TypeInfo ArrayType = typeof(IData[]).GetTypeInfo();

        static readonly TypeInfo ListType = typeof(List<IData>).GetTypeInfo();

        public void ReplyJson(int status, object data, ushort proj = 0, bool? pub = null, int maxage = 60)
        {
            TypeInfo typ = data.GetType().GetTypeInfo();

            JsonContent cont = new JsonContent();

            if (ObjectType.IsAssignableFrom(typ))
            {
                cont.Put(null, (IData)data, proj);
            }
            else if (ArrayType.IsAssignableFrom(typ))
            {
                cont.Put(null, (IData[])data, proj);
            }
            else if (ListType.IsAssignableFrom(typ))
            {
                cont.Put(null, (List<IData>)data, proj);
            }

            // set response states
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void ReplyXml(int status, object dat, ushort proj = 0, bool? pub = null, int maxage = 60)
        {
        }


        internal async Task SendAsync()
        {
            // set connection header if absent
            SetHeaderNon("Connection", "keep-alive");

            if (Pub.HasValue)
            {
                string hv = (Pub.Value ? "public" : "private") + ", max-age=" + MaxAge;
                SetHeader("Cache-Control", hv);
            }

            // setup appropriate headers
            if (Content != null)
            {
                HttpResponse r = Response;
                r.ContentLength = Content.Size;
                r.ContentType = Content.Type;

                // cache indicators
                var dyna = Content as DynamicContent;
                if (dyna != null) // set etag
                {
                    ulong etag = dyna.ETag;
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

        public DbContext NewDbContext(IsolationLevel? level = null)
        {
            DbContext dc = new DbContext(ServiceContext, this);
            if (level != null)
            {
                dc.Begin(level.Value);
            }
            return dc;
        }
    }
}