using Greatbone;

namespace Samp
{
    /// <summary>
    /// To check access to an annotated work or action method. 
    /// </summary>
    public class UserAccessAttribute : AccessAttribute
    {
        // require a persisted principal
        readonly bool stored;

        // require operator access
        readonly short opr;

        // require supply access
        readonly short sup;

        // require group access
        readonly short grp;

        public UserAccessAttribute(short opr = 0, short sup = 0, short grp = 0)
        {
            this.stored = true;
            this.opr = opr;
            this.sup = sup;
            this.grp = grp;
        }

        public UserAccessAttribute(bool stored)
        {
            this.stored = stored;
        }

        public override bool? Check(WebContext wc, IData prin)
        {
            // if not require persisted
            if (!stored) return true;

            var o = (User) prin;

            if (o.id == 0) return null;

            // if requires center access
            if (opr > 0)
            {
                return (o.opr & opr) == opr;
            }

            if (sup > 0)
            {
                if ((o.sup & sup) != sup) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.supat == at;
                }
                return true;
            }

            if (grp > 0)
            {
                if ((o.grp & grp) != grp) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.grpat == at;
                }
                return true;
            }
            return true;
        }
    }
}