using Greatbone;

namespace Samp
{
    /// <summary>
    /// To check access to an annotated work or procedure. 
    /// </summary>
    public class UserAttribute : AuthorizeAttribute
    {
        // require a ready principal
        readonly bool ready;

        // require of operator
        readonly short opr;

        // require of admin
        readonly short adm;

        public UserAttribute(bool ready)
        {
            this.ready = ready;
        }

        public UserAttribute(short opr = 0, short adm = 0)
        {
            this.ready = true;
            this.opr = opr;
            this.adm = adm;
        }

        public override bool? Check(WebContext wc, IData prin)
        {
            // if not require ready
            if (!ready) return true;

            var o = (User) prin;

            if (o.id <= 0) return null;

            // if requires operator
            if (opr > 0)
            {
                if ((o.opr & opr) != opr) return false; // inclusive check
                return o.oprat == wc[typeof(OprVarWork)];
            }
            // if requires admin
            if (adm > 0)
            {
                return (o.adm & adm) != adm;
            }
            return true;
        }
    }
}