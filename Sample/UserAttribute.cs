using Greatbone;

namespace Samp
{
    /// <summary>
    /// To check access to an annotated work or procedure. 
    /// </summary>
    public class UserAttribute : AccessAttribute
    {
        // require a ready principal
        readonly bool ready;

        // require of operator
        readonly short opr;

        // require of admin
        readonly short adm;

        public UserAttribute(short opr = 0, short adm = 0)
        {
            this.ready = true;
            this.opr = opr;
            this.adm = adm;
        }

        public UserAttribute(bool ready)
        {
            this.ready = ready;
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
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.oprat == at;
                }
                return true;
            }
            // if requires admin
            if (adm > 0)
            {
                return (o.adm & adm) == adm;
            }
            return true;
        }
    }
}