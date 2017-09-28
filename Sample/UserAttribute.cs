using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        const short NULL = short.MinValue;

        readonly short opr;

        readonly bool adm;

        public UserAttribute(short opr = NULL, bool adm = false)
        {
            this.opr = opr;
            this.adm = adm;
        }

        public bool IsOpr => opr > 0;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (opr != NULL)
            {
                if (opr == 0) return true;
                if ((prin.opr & opr) != opr) return false; // inclusive check
                return prin.oprat == ac[typeof(ShopVarWork)];
            }
            return !adm || prin.adm;
        }
    }
}