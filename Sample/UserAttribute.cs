using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        readonly short opr;

        readonly bool spr;

        readonly bool adm;

        public UserAttribute(short opr = 0, bool spr = false, bool adm = false)
        {
            this.opr = opr;
            this.spr = spr;
            this.adm = adm;
        }

        public bool IsOpr => opr > 0;

        public bool IsAdm => spr;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (opr > 0)
            {
                if ((prin.opr & opr) != opr) return false; // inclusive check
                return prin.oprat == ac[typeof(ShopVarWork)];
            }
            if (spr)
            {
                return prin.sprat == ac[typeof(CityVarWork)];
            }
            return !adm || prin.adm;
        }
    }
}