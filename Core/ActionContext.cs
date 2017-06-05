using System;
using System.Data;
using System.Diagnostics;
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
        public bool Cluster { get; internal set; }

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
        Segment[] chain;

        int level; // actual number of segments

        internal void Chain(string key, Work work)
        {
            if (chain == null)
            {
                chain = new Segment[8];
            }
            chain[level++] = new Segment(key, work);
        }

        public Segment this[int pos] => pos < 0 ? chain[level + pos - 1] : chain[pos];

        public Segment this[Type workType]
        {
            get
            {
                for (int i = 0; i < level; i++)
                {
                    Segment seg = chain[i];
                    if (seg.Work.IsSubclassOf(workType)) return seg;
                }
                return default(Segment);
            }
        }

        public Segment this[Work work]
        {
            get
            {
                for (int i = 0; i < level; i++)
                {
                    Segment seg = chain[i];
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

        string path;

        public string Path => path ?? (path = Features.Get<IHttpRequestFeature>().Path);

        string uri;

        public string Uri => uri ?? (uri = string.IsNullOrEmpty(QueryString) ? Path : Path + QueryString);

        string url;

        public string Url => url ?? (url = Features.Get<IHttpRequestFeature>().Scheme + "://" + Header("Host" + Features.Get<IHttpRequestFeature>().RawTarget));

        string querystr;

        public string QueryString => querystr ?? (querystr = Features.Get<IHttpRequestFeature>().QueryString);

        string ua;

        public string Ua => ua ?? (ua = Header("User-Agent"));

        string raddr;

        public string RemoteAddr => raddr ?? (raddr = Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString());

        public bool ByBrowser => Ua?.StartsWith("Mozilla") ?? false;

        public bool ByBrowse => ByBrowser && Header("X-Requested-With") == null;

        public bool ByWeiXin => Ua?.Contains("MicroMessenger/") ?? false;

        public bool ByJQuery => Header("X-Requested-With") != null;

        // URL query 
        Form query;

        public Form Query => query ?? (query = new FormParse(QueryString).Parse());

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
                entity = ParseContent(ctyp, buffer, count, typeof(M));
            }
            return entity as M;
        }

        public async Task<D> ReadDataAsync<D>(ushort proj = 0) where D : IData, new()
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
                entity = ParseContent(ctyp, buffer, count);
            }
            IDataInput src = entity as IDataInput;
            if (src == null)
            {
                return default(D);
            }
            return src.ToData<D>(proj);
        }

        public async Task<D[]> ReadDatasAsync<D>(ushort proj = 0) where D : IData, new()
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
                entity = ParseContent(ctyp, buffer, count);
            }
            return (entity as IDataInput)?.ToDatas<D>(proj);
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

        public void SetHeaderAbsent(string name, string v)
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

        public void SetTokenCookie<P>(P prin, ushort proj) where P : class, IData, new()
        {
            ((Service<P>)Service).SetTokenCookie(this, prin, proj);
        }

        public bool InCache { get; internal set; }

        public int Status
        {
            get => Response.StatusCode;
            set => Response.StatusCode = value;
        }

        public IContent Content { get; internal set; }

        // public, no-cache or private
        public bool? Public { get; internal set; }

        /// the cached response is to be considered stale after its age is greater than the specified number of seconds.
        public int MaxAge { get; internal set; }

        public void Give(int status, IContent content = null, bool? pub = null, int maxage = 60)
        {
            Status = status;
            Content = content;
            Public = pub;
            MaxAge = maxage;
        }

        public void Give(int status, IDataInput inp, bool? pub = null, int maxage = 60)
        {
            Status = status;
            Content = inp.Dump();
            Public = pub;
            MaxAge = maxage;
        }

        public void Give(int status, string text, bool? pub = null, int maxage = 60)
        {
            StrContent cont = new StrContent(true);
            cont.Add(text);

            // set response states
            Status = status;
            Content = cont;
            Public = pub;
            MaxAge = maxage;
        }

        public void Give(int status, IData obj, ushort proj = 0, bool? pub = null, int maxage = 60)
        {
            JsonContent cont = new JsonContent(true).Put(null, obj, proj);
            Status = status;
            Content = cont;
            Public = pub;
            MaxAge = maxage;
        }

        public void Give<D>(int status, D[] arr, ushort proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            JsonContent cont = new JsonContent(true).Put(null, arr, proj);
            Status = status;
            Content = cont;
            Public = pub;
            MaxAge = maxage;
        }

        internal async Task SendAsync()
        {
            // set connection header if absent
            SetHeaderAbsent("Connection", "keep-alive");

            // cache control header
            if (Public.HasValue)
            {
                string hv = (Public.Value ? "public" : "private") + ", max-age=" + MaxAge;
                SetHeader("Cache-Control", hv);
            }

            // content check
            if (Content != null)
            {
                var dyn = Content as DynamicContent;
                if (dyn != null) // dynamic content
                {
                    // set etag
                    string etag = StrUtility.ToHex(dyn.Checksum);
                    string inm = Header("If-None-Match");
                    if (inm != null && inm == etag)
                    {
                        Status = 304; // not modified
                        return;
                    }
                    else
                    {
                        SetHeader("ETag", etag);
                    }
                }
                else // static content
                {
                    var sta = Content as StaticContent;
                    DateTime? since = HeaderDateTime("If-Modified-Since");
                    Debug.Assert(sta != null);
                    if (since != null && sta.Modified <= since)
                    {
                        Status = 304; // not modified
                        return;
                    }

                    DateTime? last = sta.Modified;
                    if (last != null)
                    {
                        SetHeader("Last-Modified", StrUtility.FormatUtcDate(last.Value));
                    }

                    if (sta.GZip)
                    {
                        SetHeader("Content-Encoding", "gzip");
                    }
                }

                // send out the content async
                Response.ContentLength = Content.Size;
                Response.ContentType = Content.Type;
                await Response.Body.WriteAsync(Content.ByteBuffer, 0, Content.Size);
            }
        }

        //
        //

        public void Dispose()
        {
            // request content buffer
            if (buffer != null)
            {
                BufferUtility.Return(buffer);
            }

            // response content caching and pool returning

            if (!InCache && Public == true)
            {
                Service.AddCachie(this);
                InCache = true;
            }

            if (!InCache)
            {
                var dcont = Content as DynamicContent;
                if (dcont != null)
                {
                    BufferUtility.Return(dcont.ByteBuffer);
                }
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