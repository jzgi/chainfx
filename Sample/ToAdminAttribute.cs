using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ToAdminAttribute : ToAttribute
    {
        public override bool Check(WebActionContext wc)
        {
            return wc.Token is Token;
        }
    }
}