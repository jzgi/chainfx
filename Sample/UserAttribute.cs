using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        bool mgr;

        short adm;

        public UserAttribute() : this(false, 0) { }

        public UserAttribute(bool mgr, short adm)
        {
            this.mgr = mgr;
            this.adm = adm;
        }

        public bool IsMgr => mgr;

        public short IsAdm => adm;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (mgr && prin.shopid == null)
            {
                return false;
            }

            if (adm != 0 && adm > prin.admin)
            {
                return false;
            }
            return true;
        }
    }
}