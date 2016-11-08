using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BrandMuxer : WebMuxer
    {
        public BrandMuxer(WebArg arg) : base(arg)
        {
        }

        public override void @default(WebContext wc, string subscpt)
        {
        }

    }

}