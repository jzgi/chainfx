using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BrandVariableDir : WebDir, IVariable
    {
        public BrandVariableDir(WebDirContext ctx) : base(ctx)
        {
        }

        public void @default(WebContext wc, string subscpt)
        {
        }

    }

}