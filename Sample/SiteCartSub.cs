using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteCartSub : WebSub
    {
        public SiteCartSub(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}