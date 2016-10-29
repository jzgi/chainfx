using Greatbone.Core;

namespace Greatbone
{
    public class ToSelfAttribute : ToAttribute
    {
        public override bool Test(WebContext wc)
        {
            return true;
        }
    }
}