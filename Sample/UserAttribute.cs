using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : RoleAttribute
    {
        readonly bool owner;

        public UserAttribute(bool owner = true)
        {
            this.owner = owner;
        }
        
        public override bool Check(WebActionContext wc)
        {
            return wc.Token is Token;
        }
    }
}