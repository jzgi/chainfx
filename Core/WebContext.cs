using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    public class WebContext
    {
        // the implementation
        readonly HttpContext _context;

        WebSub _sub;

        WebAction _action;

        internal IZone zone;

        IToken token;

        internal WebContext(HttpContext context)
        {
            _context = context;
        }

        public WebAction Action => _action;

        public HttpRequest Request => _context.Request;

        public HttpResponse Response => _context.Response;

        public void Send<T>(T outlet) where T : IOut
        {
            _context.Response.Body.WriteAsync(outlet.Buffer, outlet.Offset, outlet.Count);
        }
    }
}