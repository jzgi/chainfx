using Greatbone.Core;

namespace Greatbone.Samp
{
    public class AuthAttribute : AuthorizeAttribute
    {
        readonly short opr;

        readonly bool adm;

        public AuthAttribute(short opr = 0, bool adm = false)
        {
            this.opr = opr;
            this.adm = adm;
        }

        public bool IsOpr => opr > 0;

        public override bool Check(ActionContext ac)
        {
            if (!(ac.Principal is User prin)) return false;

            if (opr > 0)
            {
                if ((prin.opr & opr) != opr) return false; // inclusive check
                return prin.oprat == ac[typeof(OprVarWork)];
            }
            return !adm || prin.adm;
        }
    }
}