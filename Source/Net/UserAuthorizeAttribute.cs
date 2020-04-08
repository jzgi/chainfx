using System;
using CloudUn.Web;

namespace CloudUn.Net
{
    /// <summary>
    /// To implement principal authorization of access to the target resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        readonly short role;

        public UserAuthorizeAttribute(short role = 0)
        {
            this.role = role;
        }

        public override bool Do(WebContext wc)
        {
            var prin = (User) wc.Principal;

            if (prin == null) return false;

            if (role > 0)
            {
                return (prin.role & role) == role;
            }

            return true;
        }
    }
}