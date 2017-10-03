using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        readonly short opr;

        readonly bool adm;

        public UserAttribute(short opr = 0, bool adm = false)
        {
            this.opr = opr;
            this.adm = adm;
        }

        public bool IsOpr => opr > 0;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (opr > 0)
            {
                if ((prin.opr & opr) != opr) return false; // inclusive check
                return prin.oprat == ac[typeof(OprVarWork)];
            }
            return !adm || prin.adm;
        }
    }
}