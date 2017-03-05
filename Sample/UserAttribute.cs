using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : RoleAttribute
    {
        public override bool Check(ActionContext ac)
        {
            return true;
        }
    }

    public class ShopAttribute : UserAttribute
    {
        public override bool Check(ActionContext ac)
        {
            User tok = ac.Token as User;

            if (tok == null) return false;

            if (tok.shopid == null)
            {
                return false;
            }
            return true;
        }
    }

    public class StaffAttribute : UserAttribute
    {
        public readonly short jobs;

        public StaffAttribute(short jobs)
        {
            this.jobs = jobs;
        }

        public override bool Check(ActionContext ac)
        {
            User tok = ac.Token as User;

            if (tok == null) return false;

            if (jobs != 0 && (tok.jobs & jobs) == 0)
            {
                return false;
            }
            return true;
        }
    }
}