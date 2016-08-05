using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service controller.
    ///
    public class WwwService : WebService
    {
        public WwwService(WebServiceContext wsc) : base(wsc)
        {
            AddSub<WwwMySub>("my", null);
        }

        public void Show(WebContext wc)
        {
            Fame v = new Fame();
            v.key = "123";
            v.name = "luke";
            wc.Response.Output(v, 0);
        }

        public void Contact(WebContext wc)
        {
			wc.Response.StatusCode = 200;
        }
    }
}