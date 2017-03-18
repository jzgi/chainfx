using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        bool shop;

        bool admin;

        bool sa;

        public UserAttribute() : this(false, false, false) { }

        public UserAttribute(bool shop, bool admin, bool sa)
        {
            this.shop = shop;
            this.admin = admin;
            this.sa = sa;
        }

        public bool Shop => shop;

        public bool Admin => admin;

        protected internal override void Or(AuthorizeAttribute another)
        {
            var ua = another as UserAttribute;
            if (ua == null) return;

            shop |= ua.shop;
            admin |= ua.admin;
        }

        protected internal override bool Check(ActionContext ac)
        {

            User tok = ac.Principal as User;

            if (tok == null) return false;

            if (shop && tok.shopid == null)
            {
                return false;
            }

            if (admin && tok.admin)
            {
                return false;
            }
            return true;
        }
    }
}