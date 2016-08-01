using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteCartSub : WebSub
    {
        public SiteCartSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}