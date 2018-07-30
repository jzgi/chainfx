using Greatbone;

namespace Samp
{
    /// <summary>
    /// To check access to an annotated work or action method. 
    /// </summary>
    public class UserAccessAttribute : AccessAttribute
    {
        // require a persisted principal
        readonly bool full;

        // require operator access
        readonly short ctr;

        // require supply access
        readonly short sup;

        // require group access
        readonly short grp;

        public UserAccessAttribute(short ctr = 0, short sup = 0, short grp = 0)
        {
            this.full = true;
            this.ctr = ctr;
            this.sup = sup;
            this.grp = grp;
        }

        public UserAccessAttribute(bool full)
        {
            this.full = full;
        }

        public override bool? Check(WebContext wc, IData prin)
        {
            // if not require persisted
            if (!full) return true;

            var o = (User) prin;

            if (o.id == 0) return null;

            // if requires center access
            if (ctr > 0)
            {
                return (o.opr & ctr) == ctr;
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