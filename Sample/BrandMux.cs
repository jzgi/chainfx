using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BrandMux : WebMux
    {
        public BrandMux(WebNodeContext wnc) : base(wnc)
        {
        }

        public override void @default(WebContext wc, string subscpt)
        {
        }

    }

}