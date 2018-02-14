using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Greatbone.Core
{
    public class ActionRequest : HttpRequest
    {
        private readonly HttpContext context;

        readonly IHttpRequestFeature fRequest;

        readonly IRequestCookiesFeature fCookies;

        internal ActionRequest(HttpContext context)
        {
            this.context = context;
            fRequest = context.Features.Get<IHttpRequestFeature>();
            fCookies = context.Features.Get<IRequestCookiesFeature>();
        }

        public override HttpContext HttpContext => context;

        public override string Method
        {
            get => fRequest.Method;
            set => fRequest.Method = value;
        }

        public override string Scheme
        {
            get => fRequest.Scheme;
            set => fRequest.Scheme = value;
        }

        public override bool IsHttps
        {
            get => fRequest.Scheme == "https";
            set => fRequest.Scheme = value ? "https" : "http";
        }

        public override HostString Host
        {
            get => throw new Exception();
            set => throw new Exception();
        }

        public override PathString PathBase
        {
            get => new PathString(fRequest.PathBase);
            set => fRequest.PathBase = value.Value;
        }

        public override PathString Path
        {
            get => new PathString(fRequest.Path);
            set => fRequest.Path = value.Value;
        }

        public override QueryString QueryString
        {
            get => throw new Exception();
            set => throw new Exception();
        }

        public override IQueryCollection Query { get; set; }

        public override string Protocol
        {
            get => fRequest.Protocol;
            set => fRequest.Protocol = value;
        }

        public override IHeaderDictionary Headers => fRequest.Headers;

        public override IRequestCookieCollection Cookies
        {
            get => fCookies.Cookies;
            set => throw null;
        }

        public override long? ContentLength
        {
            get
            {
                if (Headers.TryGetValue("Content-Length", out var vs))
                {
                    return long.Parse(vs[0]);
                }

                return null;
            }
            set => Headers.Add("Content-Length", value.ToString());
        }

        public override string ContentType
        {
            get => Headers["Content-Type"];
            set => Headers["Content-Type"] = value;
        }

        public override Stream Body
        {
            get => fRequest.Body;
            set => fRequest.Body = value;
        }

        [Obsolete]
        public override bool HasFormContentType { get; } = false;

        [Obsolete]
        public override IFormCollection Form { get; set; } = null;

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}