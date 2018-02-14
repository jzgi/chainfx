using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    public class ActionResponse : HttpResponse
    {
        readonly ActionContext context;

        readonly IHttpResponseFeature fResponse;

        readonly IResponseCookiesFeature fCookies;

        internal ActionResponse(ActionContext contex)
        {
            this.context = contex;
            fResponse = contex.Features.Get<IHttpResponseFeature>();
            fCookies = contex.Features.Get<IResponseCookiesFeature>();
        }

        public override HttpContext HttpContext => context;

        public override int StatusCode
        {
            get => fResponse.StatusCode;
            set => fResponse.StatusCode = value;
        }

        public override IHeaderDictionary Headers => fResponse.Headers;

        public override Stream Body
        {
            get => fResponse.Body;
            set => fResponse.Body = value;
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

        public override IResponseCookies Cookies => fCookies.Cookies;

        public override bool HasStarted => fResponse.HasStarted;

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            fResponse.OnStarting(callback, state);
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            fResponse.OnCompleted(callback, state);
        }

        public override void Redirect(string location, bool permanent)
        {
            fResponse.StatusCode = !permanent ? 302 : 301;
            Headers["Location"] = (StringValues) location;
        }
    }
}