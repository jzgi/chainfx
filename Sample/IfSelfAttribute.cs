using Greatbone.Core;

namespace Greatbone
{
    public class IfSelfAttribute : IfAttribute
    {
        public override bool Test(WebContext wc)
        {
            return true;
        }
    }
}