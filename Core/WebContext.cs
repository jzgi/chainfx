using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Greatbone.Core
{
    ///
    /// The encapsulation of a web request/response exchange context.
    ///
    /// buffer pooling -- reduces GC overhead when dealing with asynchronous request/response I/O
    ///
    public class WebContext : DefaultHttpContext
    {
        internal WebContext(IFeatureCollection features) : base(features)
        {
        }

        protected override HttpRequest InitializeHttpRequest() => new WebRequest(this);

        protected override HttpResponse InitializeHttpResponse() => new WebResponse(this);

        public new WebRequest Request => (WebRequest) base.Request;

        public new WebResponse Response => (WebResponse) base.Response;

        public WebSub Controller { get; }

        public WebAction Action { get; }

        public string X { get; internal set; }

        public IToken Token { get; }

        public void Dispose()
        {
        }
    }
}