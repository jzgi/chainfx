using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        public readonly bool shopop;

        public readonly short jobs;

        public UserAttribute(bool shopop = false, short jobs = 0)
        {
            this.shopop = shopop;
            this.jobs = jobs;
        }

        public override bool Check(ActionContext ac)
        {
            User tok = ac.Token as User;

            if (tok == null) return false;

            if (shopop)
            {
                if (tok.shopid == null)
                {
                    return false;
                }
            }
            if (jobs != 0 && (tok.jobs & jobs) == 0)
            {
                return false;
            }
            return true;
        }
    }
}