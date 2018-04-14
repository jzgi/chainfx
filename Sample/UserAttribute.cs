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

        public override bool Allowed(IData prin, WebContext wc)
        {
            var o = (User) prin;
            if (opr > 0)
            {
                if ((o.opr & opr) != opr) return false; // inclusive check
                return o.oprat == wc[typeof(OprVarWork)];
            }
            if (adm > 0)
            {
                return (o.adm & adm) != adm;
            }
            return true;
        }
    }
}