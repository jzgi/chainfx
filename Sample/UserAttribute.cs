using Greatbone;

namespace Core
{
    /// <summary>
    /// To check access to an annotated work or procedure. 
    /// </summary>
    public class UserAttribute : AuthorizeAttribute
    {
        readonly short opr;

        readonly short adm;

        public UserAttribute(short opr = 0, short adm = 0)
        {
            this.opr = opr;
            this.adm = adm;
        }

        public bool IsOpr => opr > 0;

        public override bool Check(WebContext wc)
        {
            if (!(wc.Principal is User prin)) return false;

            if (opr > 0)
            {
                if ((prin.opr & opr) != opr) return false; // inclusive check
                return prin.oprat == wc[typeof(OprVarWork)];
            }
            return (prin.adm & adm) != adm;
        }
    }
}