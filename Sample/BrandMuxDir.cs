using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BrandMuxDir : WebDir, IMux
    {
        public BrandMuxDir(WebDirContext ctx) : base(ctx)
        {
        }

        public void @default(WebContext wc, string subscpt)
        {
        }

    }

}