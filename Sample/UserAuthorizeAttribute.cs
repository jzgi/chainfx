using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// To implement principal authorization of access to the target resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        // required hub access
        readonly short hubly;

        // required shop access
        readonly short shoply;

        // required customer team access
        readonly short teamly;

        public UserAuthorizeAttribute(short hubly = 0, short shoply = 0, short teamly = 0)
        {
            this.hubly = hubly;
            this.shoply = shoply;
            this.teamly = teamly;
        }

        public override bool Do(WebContext wc)
        {
            var o = (User) wc.Principal;

            if (o == null || o.IsTemporary) return false;

            // if requires hub access
            if (hubly > 0)
            {
                return (o.hubly & hubly) > 0;
            }
            // if requires shop access
            if (shoply > 0)
            {
                if ((o.teamly & shoply) != shoply) return false; // inclusive check
                short at = wc[typeof(IOrgVar)];
                if (at != 0)
                {
                    return o.shopid == at;
                }
                return true;
            }
            // if requires customer team access
            if (teamly > 0)
            {
                if ((o.teamly & teamly) != teamly) return false; // inclusive check
                short at = wc[typeof(IOrgVar)];
                if (at != 0)
                {
                    return o.teamid == at;
                }
                return true;
            }
            return true;
        }
    }
}