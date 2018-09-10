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

        // required hub access
        readonly short hub;

        // required shop access
        readonly short shop;

        // required customer team access
        readonly short team;

        public UserAccessAttribute(short hub = 0, short shop = 0, short team = 0)
        {
            this.full = true;
            this.hub = hub;
            this.shop = shop;
            this.team = team;
        }

        public UserAccessAttribute(bool full)
        {
            this.full = full;
        }

        public override bool? Check(WebContext wc)
        {
            var o = (User) wc.Principal;

            if (o == null) return false;

            // if not require persisted
            if (!full) return true;

            // info incomplete
            if (o.name == null || o.tel == null || o.addr == null) return null;

            // if requires hub access
            if (hub > 0)
            {
                return (o.hub & hub) > 0;
            }
            // if requires shop access
            if (shop > 0)
            {
                if ((o.team & shop) != shop) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.shopat == at;
                }
                return true;
            }
            // if requires customer team access
            if (team > 0)
            {
                if ((o.team & team) != team) return false; // inclusive check
                string at = wc[typeof(IOrgVar)];
                if (at != null)
                {
                    return o.teamat == at;
                }
                return true;
            }
            return true;
        }
    }
}