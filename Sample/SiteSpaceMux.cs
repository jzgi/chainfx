using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteSpaceMux : WebMux<Space>
    {
        public SiteSpaceMux(WebHub parent) : base(parent)
        {
            AddSub<SiteSpaceMgtSub>("mgt", null);
        }

        public override void Default(WebContext wc, Space zone)
        {
            throw new System.NotImplementedException();
        }
    }
}