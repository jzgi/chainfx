using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    public class WebContext
    {
        // the implementation
        private readonly HttpContext _context;

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

        public void Send<T>(T content) where T : IContent
        {
            _context.Response.Body.WriteAsync(content.Buffer, content.Offset, content.Count);
        }
    }
}