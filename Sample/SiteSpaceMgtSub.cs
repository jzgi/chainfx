using Greatbone.Core;

namespace Greatbone.Sample
{
    public class SiteSpaceMgtSub : WebSub<Space>
    {
        public SiteSpaceMgtSub(WebMux<Space> mux) : base(mux)
        {
        }

        public override void Default(WebContext wc, Space zone)
        {
            throw new System.NotImplementedException();
        }
    }
}