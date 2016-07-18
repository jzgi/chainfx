using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteCartSub : WebSub
    {
        public SiteCartSub(WebService service) : base(service)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}