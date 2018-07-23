using Greatbone;

namespace Samp
{
    /// <summary>
    /// To check access to an annotated work or action method. 
    /// </summary>
    public class UserAccessAttribute : AccessAttribute
    {
        // require a complete principal
        readonly bool complete;

        // require center access
        readonly short ctr;

        // require vendor access
        readonly short vdr;

        // require team access
        readonly short tm;

        // require platform access
        readonly short plat;

        public UserAccessAttribute(short ctr = 0, short vdr = 0, short tm = 0, short plat = 0)
        {
            this.complete = true;
            this.ctr = ctr;
            this.vdr = vdr;
            this.tm = tm;
            this.plat = plat;
        }

        public UserAccessAttribute(bool complete)
        {
            this.complete = complete;
        }

        public override bool? Check(WebContext wc, IData prin)
        {
            // if not require complete
            if (!complete) return true;

            var o = (User) prin;

            if (o.id == 0) return null;

            // if requires center access
            if (ctr > 0)
            {
                if ((o.ctr & ctr) != ctr) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.ctrat == at;
                }
                return true;
            }
            // if requires vendor access
            if (vdr > 0)
            {
                if ((o.vdr & vdr) != vdr) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.vdrat == at;
                }
                return true;
            }
            // if requires team access
            if (tm > 0)
            {
                if ((o.tm & tm) != tm) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.tmat == at;
                }
                return true;
            }
            // if requires platform access
            if (plat > 0)
            {
                return (o.plat & plat) == plat;
            }
            return true;
        }
    }
}