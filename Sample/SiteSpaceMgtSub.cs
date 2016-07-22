using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteSpaceMgtSub : WebSub<Space>
    {
        public SiteSpaceMgtSub(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc, Space zone)
        {
            throw new System.NotImplementedException();
        }
    }
}