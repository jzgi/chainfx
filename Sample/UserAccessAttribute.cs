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

        // require group access
        readonly short grp;

        public UserAccessAttribute(short ctr = 0, short grp = 0)
        {
            this.full = true;
            this.ctr = ctr;
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

            // info incomplete
            if (o.name == null || o.tel == null) return null; 

            // if requires center access
            if (ctr > 0)
            {
                return (o.ctr & ctr) == ctr;
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