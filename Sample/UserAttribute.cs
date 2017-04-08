using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        bool opr;

        short adm;

        public UserAttribute() : this(false, 0) { }

        public UserAttribute(bool opr, short adm)
        {
            this.opr = opr;
            this.adm = adm;
        }

        public bool IsOpr => opr;

        public short IsAdm => adm;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (opr && prin.shopid == null)
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