using Greatbone.Core;

namespace Greatbone
{
    public class IfSelfAttribute : IfAttribute
    {
        public override bool Check(WebContext wc, string subscpt)
        {
            return true;
        }
    }
}