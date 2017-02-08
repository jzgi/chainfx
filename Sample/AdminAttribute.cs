using Greatbone.Core;

namespace Greatbone.Sample
{
    public class AdminAttribute : RoleAttribute
    {
        public override bool Check(WebActionContext wc)
        {
            return wc.Token is Token;
        }
    }
}