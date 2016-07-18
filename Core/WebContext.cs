using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
  public class WebContext
  {
    // the implementation
    private readonly HttpContext _context;

    private WebRequest _request;

    private WebResponse _response;

    WebSub _sub;

    WebAction _action;

    internal IZone zone;

    IToken token;

    internal WebContext(HttpContext context)
    {
      _context = context;
      _request = new WebRequest(context.Request);
      _response = new WebResponse(context.Response);
    }

    public WebAction Action => _action;

    public WebRequest Request => _request;

    public WebResponse Response => _response;

    public void Send<T>(T content) where T : IContent
    {
      _context.Response.Body.WriteAsync(content.Buffer, content.Offset, content.Count);
    }
  }
}