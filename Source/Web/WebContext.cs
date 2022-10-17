#pragma warning disable 618
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using static ChainFx.DataUtility;
using AuthenticationManager = Microsoft.AspNetCore.Http.Authentication.AuthenticationManager;

namespace ChainFx.Web
{
    ///
    /// The encapsulation of a web request/response exchange context. It supports multiplexity occuring in SSE and WebSocket.
    ///
    public sealed class WebContext : HttpContext
    {
        readonly IFeatureCollection features;

        private readonly DefaultConnectionInfo connection;

        readonly IHttpRequestFeature fRequest;

        readonly RequestCookiesFeature fRequestCookies;

        readonly IHttpResponseFeature fResponse;

        readonly IHttpWebSocketFeature fWebSocket;

        internal WebContext(IFeatureCollection features)
        {
            this.features = features;
            connection = new DefaultConnectionInfo(features);
            fRequest = features.Get<IHttpRequestFeature>();
            fRequestCookies = new RequestCookiesFeature(features);
            fResponse = features.Get<IHttpResponseFeature>();
            fWebSocket = features.Get<IHttpWebSocketFeature>();
        }

        public WebService Service { get; internal set; }

        public WebWork Work => segnum == 0 ? Service : segs[segnum - 1].Work;

        public WebAction Action { get; internal set; }

        public int Subscript { get; set; }

        public Exception Error { get; set; }

        /// The decrypted/decoded principal object.
        ///
        public IData Principal { get; set; }

        public short Role { get; set; }

        //
        // the segment stack
        //

        // segments along the URI path
        WebSeg[] segs;

        int segnum; // actual number of segments

        internal void AppendSeg(WebWork work, string key, object accessor = null)
        {
            if (segs == null)
            {
                segs = new WebSeg[8];
            }

            segs[segnum++] = new WebSeg(work, key, accessor);
        }

        public WebSeg this[int pos] => pos <= 0 ? segs[segnum + pos - 1] : default;

        public WebSeg this[Type typ]
        {
            get
            {
                for (int i = segnum - 1; i >= 0; i--)
                {
                    var seg = segs[i];
                    if (seg.Work.IsOf(typ)) return seg;
                }

                return default;
            }
        }

        public WebSeg this[WebWork work]
        {
            get
            {
                for (int i = segnum - 1; i >= 0; i--)
                {
                    var seg = segs[i];
                    if (seg.Work == work) return seg;
                }

                return default;
            }
        }

        // WEB FEATURES

        public override IFeatureCollection Features => features;

        public override HttpRequest Request => null;

        public override HttpResponse Response => null;

        public override ConnectionInfo Connection => connection;

        public override WebSocketManager WebSockets => null;

        public bool IsWebSocketRequest => fWebSocket.IsWebSocketRequest;

        public Task<WebSocket> AcceptWebSocketAsync() => fWebSocket.AcceptAsync(null);

        [Obsolete]
        public override AuthenticationManager Authentication => null;

        public override ClaimsPrincipal User { get; set; } = null;

        public override IDictionary<object, object> Items { get; set; } = null;

        public override IServiceProvider RequestServices { get; set; } = null;

        public override CancellationToken RequestAborted
        {
            get => features.Get<IHttpRequestLifetimeFeature>().RequestAborted;
            set { }
        }

        public override void Abort() => features.Get<IHttpRequestLifetimeFeature>().Abort();

        public override string TraceIdentifier { get; set; } = null;

        public override ISession Session { get; set; } = null;


        //
        // REQUEST
        //

        public string Method => fRequest.Method;

        public bool IsGet => "GET".Equals(fRequest.Method);

        public bool IsPost => "POST".Equals(fRequest.Method);

        public string UserAgent => Header("User-Agent");

        public IPAddress RemoteIpAddress => connection.RemoteIpAddress;

        public bool IsWeChat => UserAgent?.Contains("MicroMessenger/") ?? false;

        public bool IsAjax => Header("X-Requested-With") != null;

        public string Path => fRequest.Path;

        string uri;

        public string Uri => uri ??= string.IsNullOrEmpty(QueryStr) ? Path : Path + QueryStr;

        string url;

        public string Url => url ??= fRequest.Scheme + "://" + Header("Host") + fRequest.RawTarget;

        public string QueryStr => fRequest.QueryString;

        // URL query 
        Form query;

        public Form Query => query ??= new FormParser(QueryStr).Parse();

        public void AddQueryPair(string name, string value)
        {
            string q = fRequest.QueryString;
            if (string.IsNullOrEmpty(q))
            {
                fRequest.QueryString = "?" + name + "=" + value;
                query = null; // reset parsed form
            }
            else
            {
                fRequest.QueryString = fRequest.QueryString + "&" + name + "=" + value;
                Query.Add(name, value);
            }
        }

        //
        // HEADER
        //

        public string Header(string name)
        {
            if (fRequest.Headers.TryGetValue(name, out var vs))
            {
                return vs;
            }

            return null;
        }

        public short? HeaderShort(string name)
        {
            if (fRequest.Headers.TryGetValue(name, out var vs))
            {
                string str = vs;
                if (short.TryParse(str, out var v))
                {
                    return v;
                }
            }

            return null;
        }

        public int? HeaderInt(string name)
        {
            if (fRequest.Headers.TryGetValue(name, out var vs))
            {
                string str = vs;
                if (int.TryParse(str, out var v))
                {
                    return v;
                }
            }

            return null;
        }

        public long? HeaderLong(string name)
        {
            if (fRequest.Headers.TryGetValue(name, out var vs))
            {
                string str = vs;
                if (long.TryParse(str, out var v))
                {
                    return v;
                }
            }

            return null;
        }

        public DateTime? HeaderDateTime(string name)
        {
            if (fRequest.Headers.TryGetValue(name, out var vs))
            {
                string str = vs;
                if (TextUtility.TryParseUtcDate(str, out var v))
                {
                    return v;
                }
            }

            return null;
        }

        public decimal? HeaderDecimal(string name)
        {
            if (fRequest.Headers.TryGetValue(name, out var vs))
            {
                string str = vs;
                if (decimal.TryParse(str, out var v))
                {
                    return v;
                }
            }

            return null;
        }

        public string[] Headers(string name)
        {
            if (fRequest.Headers.TryGetValue(name, out var vs))
            {
                return vs;
            }

            return null;
        }

        public IRequestCookieCollection Cookies => fRequestCookies.Cookies;

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
                    int len = (int) clen;
                    buffer = new byte[len];
                    while ((count += await fRequest.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }
            }

            return new ArraySegment<byte>(buffer, 0, count);
        }

        public async Task<M> ReadAsync<M>() where M : class, ISource
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    int len = (int) clen;
                    buffer = new byte[len];
                    while ((count += await fRequest.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }

                // parse
                string ctyp = Header("Content-Type");
                entity = ParseContent(ctyp, buffer, count, typeof(M));
            }

            return entity as M;
        }

        public async Task<D> ReadObjectAsync<D>(short proj = 0xff, D instance = default) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    int len = (int) clen;
                    buffer = new byte[len];
                    while ((count += await fRequest.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }

                // parse
                string ctyp = Header("Content-Type");
                entity = ParseContent(ctyp, buffer, count);
            }

            if (!(entity is ISource src))
            {
                return default;
            }

            if (instance == null)
            {
                instance = new D();
            }

            instance.Read(src, proj);
            return instance;
        }

        public async Task<D[]> ReadArrayAsync<D>(short proj = 0xff) where D : IData, new()
        {
            if (entity == null && count == -1) // if not yet parse and read
            {
                // read
                count = 0;
                int? clen = HeaderInt("Content-Length");
                if (clen > 0)
                {
                    int len = (int) clen;
                    buffer = new byte[len];
                    while ((count += await fRequest.Body.ReadAsync(buffer, count, (len - count))) < len)
                    {
                    }
                }

                // parse
                string ctyp = Header("Content-Type");
                entity = ParseContent(ctyp, buffer, count);
            }

            return (entity as ISource)?.ToArray<D>(proj);
        }

        //
        // RESPONSE
        //

        public void SetHeader(string name, int v)
        {
            fResponse.Headers.Add(name, new StringValues(v.ToString()));
        }

        public void SetHeader(string name, long v)
        {
            fResponse.Headers.Add(name, new StringValues(v.ToString()));
        }

        public void SetHeader(string name, string v)
        {
            fResponse.Headers.Add(name, new StringValues(v));
        }

        public void SetHeaderAbsent(string name, string v)
        {
            var headers = fResponse.Headers;
            if (!headers.TryGetValue(name, out _))
            {
                headers.Add(name, new StringValues(v));
            }
        }

        public void SetHeader(string name, DateTime v)
        {
            var str = TextUtility.FormatUtcDate(v);
            fResponse.Headers.Add(name, new StringValues(str));
        }

        public void SetHeader(string name, params string[] values)
        {
            fResponse.Headers.Add(name, new StringValues(values));
        }

        public void SetCookie(string name, string value, int maxage = 0, string domain = null, string path = "/")
        {
            var sb = new StringBuilder(name).Append('=').Append(value);
            if (maxage > 0)
            {
                sb.Append("; Max-Age=").Append(maxage);
            }

            if (domain != null)
            {
                sb.Append("; Domain=").Append(domain);
            }

            if (path != null)
            {
                sb.Append("; Path=").Append(path);
            }

            sb.Append("; HttpOnly");
            SetHeader("Set-Cookie", sb.ToString());
        }

        //
        // RESPONSE

        public int StatusCode
        {
            get => fResponse.StatusCode;
            set => fResponse.StatusCode = value;
        }

        public IContent Content { get; internal set; }

        // public, no-cache or private
        public bool? Shared { get; internal set; }

        /// the cached response is to be considered stale after its age is greater than the specified number of seconds.
        public int MaxAge { get; internal set; }

        /// <summary>
        /// RFC 7231 cacheable status codes.
        /// </summary>
        public bool IsCacheable()
        {
            var sc = StatusCode;
            return sc == 200 || sc == 203 || sc == 204 || sc == 206 || sc == 300 || sc == 301 || sc == 404 || sc == 405 || sc == 410 || sc == 414 || sc == 501;
        }

        public void Give(int statusCode, IContent content = null, bool? shared = null, int maxage = 12)
        {
            StatusCode = statusCode;
            Content = content;
            Shared = shared;
            MaxAge = maxage;
        }

        public void GiveMsg(int statusCode, string msg, string detail = null, bool? shared = null, int maxage = 12)
        {
            var cnt = new TextBuilder(true, 1024);

            cnt.Add(msg);
            if (detail != null)
            {
                cnt.Add('\n');
                cnt.Add(detail);
            }

            StatusCode = statusCode;
            Content = cnt;
            Shared = shared;
            MaxAge = maxage;
        }

        internal async Task SendAsync()
        {
            // set connection header if absent
            SetHeaderAbsent("Connection", "keep-alive");

            // cache control header
            if (Shared.HasValue)
            {
                var hv = (Shared.Value ? "public" : "private") + ", max-age=" + MaxAge;
                SetHeader("Cache-Control", hv);
            }

            // content check
            if (Content == null) return;

            // deal with not modified situations by etag
            var etag = Content.ETag;
            if (etag != null)
            {
                var inm = Header("If-None-Match");
                if (etag == inm)
                {
                    StatusCode = 304; // not modified
                    return;
                }

                SetHeader("ETag", etag);
            }

            // static content special deal
            if (Content is StaticResource sta)
            {
                var since = HeaderDateTime("If-Modified-Since");
                Debug.Assert(sta != null);
                if (since != null && sta.Adapted <= since)
                {
                    StatusCode = 304; // not modified
                    return;
                }

                var last = sta.Adapted;
                if (last != null)
                {
                    SetHeader("Last-Modified", TextUtility.FormatUtcDate(last.Value));
                }

                if (sta.GZip)
                {
                    SetHeader("Content-Encoding", "gzip");
                }
            }

            // send out the content async
            fResponse.Headers["Content-Length"] = Content.Count.ToString();
            fResponse.Headers["Content-Type"] = Content.CType;
            await fResponse.Body.WriteAsync(Content.Buffer, 0, Content.Count);
        }

        /// <summary>
        /// Called on context closing 
        /// </summary>
        public void Close()
        {
            if (Content is DynamicBuilder dyn)
            {
                dyn.Clear();
            }
        }
    }
}