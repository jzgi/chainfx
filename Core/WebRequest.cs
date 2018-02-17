using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Greatbone.Core
{
    public class WebRequest : HttpRequest
    {
        private readonly HttpContext context;

        readonly IHttpRequestFeature fRequest;

        readonly RequestCookiesFeature fCookies;

        internal WebRequest(HttpContext context)
        {
            this.context = context;
            var features = context.Features;
            fRequest = features.Get<IHttpRequestFeature>();
            fCookies = new RequestCookiesFeature(features);
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
            get => new HostString(Headers["Host"]);
            set => throw new Exception();
        }

        public override PathString PathBase
        {
            get => new PathString(fRequest.PathBase);
            set => fRequest.PathBase = value.Value;
        }

        [Obsolete("We need plain string only")]
        public override PathString Path { get; set; }

        public string PathStr => fRequest.Path;

        public string RawTarget => fRequest.RawTarget;

        public string QueryStr
        {
            get => fRequest.QueryString;
            set => fRequest.QueryString = value;
        }

        static readonly Exception NotImplementedEx = new NotImplementedException();

        // we need plain string only
        public override QueryString QueryString
        {
            get => throw NotImplementedEx;
            set => throw NotImplementedEx;
        }


        // we parse query string by ourselves
        public override IQueryCollection Query
        {
            get => throw NotImplementedEx;
            set => throw NotImplementedEx;
        }

        public override string Protocol
        {
            get => fRequest.Protocol;
            set => fRequest.Protocol = value;
        }

        public override IHeaderDictionary Headers => fRequest.Headers;

        public override IRequestCookieCollection Cookies
        {
            get => fCookies.Cookies;
            set => fCookies.Cookies = value;
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

        // we parse form by ourselves
        public override bool HasFormContentType => throw NotImplementedEx;

        // we parse form by ourselves
        public override IFormCollection Form
        {
            get => throw NotImplementedEx;
            set => throw NotImplementedEx;
        }

        // we parse form by ourselves
        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken()) => throw NotImplementedEx;
    }
}