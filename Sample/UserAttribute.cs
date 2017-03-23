using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        bool shopmgr;

        short admin;

        public UserAttribute() : this(false, 0) { }

        public UserAttribute(bool shopmgr, short admin)
        {
            this.shopmgr = shopmgr;
            this.admin = admin;
        }

        public bool ShopMgr => shopmgr;

        public short Admin => admin;

        public override void Or(AuthorizeAttribute another)
        {
            var ua = another as UserAttribute;
            if (ua == null) return;

            shopmgr |= ua.shopmgr;
            admin |= ua.admin;
        }

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (shopmgr && prin.shopid == null)
            {
                return false;
            }

            if (admin != 0 && admin > prin.admin)
            {
                return false;
            }
            return true;
        }
    }
}