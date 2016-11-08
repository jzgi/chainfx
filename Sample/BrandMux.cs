using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BrandMux : WebMux
    {
        public BrandMux(WebHierarchyContext whc) : base(whc)
        {
        }

        public override void @default(WebContext wc, string subscpt)
        {
        }

    }

}