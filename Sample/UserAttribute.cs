using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        readonly short opr;

        readonly short adm;

        public UserAttribute() : this(0, 0)
        {
        }

        public UserAttribute(short opr, short adm)
        {
            this.opr = opr;
            this.adm = adm;
        }

        public bool IsOpr => opr > 0;

        public bool IsAdm => adm > 0;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (opr != 0)
            {
                if ((prin.opr & opr) == 0 || prin.oprshopid == null)
                {
                    return false;
                }
                string shopid = ac[typeof(ShopVarWork)];
                return shopid == prin.oprshopid;
            }
            if (adm != 0)
            {
                if ((prin.adm & adm) == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}