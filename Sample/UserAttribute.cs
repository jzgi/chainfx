using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        readonly bool opr;

        readonly bool dvr;

        readonly bool mgr;

        readonly bool adm;

        public UserAttribute(bool opr = false, bool dvr = false, bool mgr = false, bool adm = false)
        {
            this.opr = opr;
            this.dvr = dvr;
            this.mgr = mgr;
            this.adm = adm;
        }

        public bool IsOpr => opr;

        public bool IsAdm => mgr;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (opr)
            {
                if (prin.oprat == null) return false;
                return prin.oprat == ac[typeof(ShopVarWork)];
            }
            if (dvr)
            {
                if (prin.dvrat == null) return false;
                return prin.dvrat == ac[typeof(ShopVarWork)];
            }
            if (mgr)
            {
                if (prin.mgrat == null) return false;
                return prin.mgrat == ac[typeof(CityVarWork)];
            }
            return !adm || prin.adm;
        }
    }
}