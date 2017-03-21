using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        bool shop;

        short admin;

        public UserAttribute() : this(false, 0) { }

        public UserAttribute(bool shop, short admin)
        {
            this.shop = shop;
            this.admin = admin;
        }

        public bool Shop => shop;

        public short Admin => admin;

        public override void Or(AuthorizeAttribute another)
        {
            var ua = another as UserAttribute;
            if (ua == null) return;

            shop |= ua.shop;
            admin |= ua.admin;
        }

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (shop && prin.shopid == null)
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