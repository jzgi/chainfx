using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AccessAttribute
    {
        readonly bool owner;

        public UserAttribute(bool owner = true)
        {
            this.owner = owner;
        }

        public override bool Check(ActionContext wc)
        {
            return ((Token)wc.Token).IsUser;
        }
    }
}