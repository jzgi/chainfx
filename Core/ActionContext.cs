using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using static Greatbone.Core.DataInputUtility;

namespace Greatbone.Core
{
    ///
    /// The encapsulation of a web request/response exchange context.
    ///
    public class ActionContext : DefaultHttpContext, IDoerContext<ActionInfo>, IDisposable
    {
        internal ActionContext(IFeatureCollection features) : base(features)
        {
        }

        public Service Service { get; set; }

        /// Whether this is requested from a cluster member.
        ///
        public bool Clustered { get; internal set; }

        public Work Work { get; internal set; }

        public ActionInfo Doer { get; internal set; }

        public int Subscript { get; internal set; }

        /// The decrypted/decoded principal object.
        ///
        public IData Principal { get; internal set; }

        /// A token string.
        ///
        public string Token { get; internal set; }

        // levels of keys along the URI path
        Segment[] segs;

        int segnum; // actual number of knots

        internal void Chain(string key, Work work)
        {
            if (segs == null)
            {
                segs = new Segment[4];
            }
            segs[segnum++] = new Segment(key, work);
        }

        public Segment this[int level] => segs[level];

        public Segment this[Type workType]
        {
            get
            {
                for (int i = 0; i < segnum; i++)
                {
                    Segment seg = segs[i];
                    if (seg.Type == workType) return seg;
                }
                return default(Segment);
            }
        }

        public Segment this[Work work]
        {
            get
            {
                for (int i = 0; i < segnum; i++)
                {
                    Segment seg = segs[i];
                    if (seg.Work == work) return seg;
                }
                return default(Segment);
            }
        }

        //
        // REQUEST
        //

        public string Method => Request.Method;

        public bool GET => "GET".Equals(Request.Method);

        public bool POST => "POST".Equals(Request.Method);

        string uri;

        public string Uri
        {
            get
            {
                if (uri == null)
                {
                    uri = Features.Get<IHttpRequestFeature>().RawTarget;
                }
                return uri;
            }
        }

        string querystr;

        public string QueryString
        {
            get
            {
                if (querystr == null)
                {
                    querystr = Features.Get<IHttpRequestFeature>().QueryString;
                }
                return querystr;
            }
        }

        string ua;

        public string Ua
        {
            get
            {
                if (ua == null)
                {
                    ua = Header("User-Agent");
                }
                return ua;
            }
        }

        public bool ByBrowser => Ua?.StartsWith("Mozilla") ?? false;

        public bool ByBrowse => ByBrowser && Header("X-Requested-With") == null;

        public bool ByWeiXin => Ua?.Contains("MicroMessenger/") ?? false;

        public bool ByJquery => Header("X-Requested-With") != null;

        // URL query 
        Form query;

        public Form Query
        {
            get
            {
                if (query == null)
                {
                    FormParse p = new FormParse(QueryString);
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

        public string[] Headers(string name)
        {
            StringValues vs;
            if (Request.Headers.TryGetValue(name, out vs))
            {
                return vs;
            }
            return null;
        }

        public IRequestCookieCollection Cookies => Request.Cookies;

        // request body
        byte[] buffer;

        int count = -1;

        // request entity (ArraySegment<byte>, JObj, JArr, Form, XElem, null)
        object entity;

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
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len) { }
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
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len) { }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = ParseContent(ctyp, buffer, count, typeof(M));
            }
            return entity as M;
        }

        public async Task<D> ReadObjectAsync<D>(int proj = 0) where D : IData, new()
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
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len) { }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = ParseContent(ctyp, buffer, count);
            }
            IDataInput src = entity as IDataInput;
            if (src == null)
            {
                return default(D);
            }
            return src.ToObject<D>(proj);
        }

        public async Task<D[]> ReadArrayAsync<D>(int proj = 0) where D : IData, new()
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
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len) { }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = ParseContent(ctyp, buffer, count);
            }
            return (entity as IDataInput)?.ToArray<D>(proj);
        }

        public async Task<List<D>> ReadListAsync<D>(int proj = 0) where D : IData, new()
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
                    while ((count += await Request.Body.ReadAsync(buffer, count, (len - count))) < len) { }
                }
                // parse
                string ctyp = Header("Content-Type");
                entity = ParseContent(ctyp, buffer, count);
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

        public void SetHeader(string name, long v)
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

        public void Give(int status, IContent content = null, bool? pub = null, int maxage = 60)
        {
            Status = status;
            Content = content;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Give(int status, IDataInput inp, bool? pub = null, int maxage = 60)
        {
            Status = status;
            Content = inp.Dump();
            Pub = pub;
            MaxAge = maxage;
        }

        public void Give(int status, string text, bool? pub = null, int maxage = 60)
        {
            TextContent cont = new TextContent(true);
            cont.Add(text);

            // set response states
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Give(int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            JsonContent cont = new JsonContent().Put(null, obj, proj);
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Give<D>(int status, D[] arr, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            JsonContent cont = new JsonContent().Put(null, arr, proj);
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
        }

        public void Give<D>(int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            JsonContent cont = new JsonContent().Put(null, lst, proj);
            Status = status;
            Content = cont;
            Pub = pub;
            MaxAge = maxage;
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
            DbContext dc = new DbContext(Service, this);
            if (level != null)
            {
                dc.Begin(level.Value);
            }
            return dc;
        }
    }
}