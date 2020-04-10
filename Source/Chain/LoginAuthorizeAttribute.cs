using System;
using SkyCloud.Web;

namespace SkyCloud.Chain
{
    /// <summary>
    /// To implement principal authorization of access to the target resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class LoginAuthorizeAttribute : AuthorizeAttribute
    {
        readonly short role;

        public LoginAuthorizeAttribute(short role = 0)
        {
            this.role = role;
        }

        public override bool Do(WebContext wc)
        {
            var prin = (Login) wc.Principal;

            if (prin == null) return false;

            if (role > 0)
            {
                return (prin.typ & role) == role;
            }

            return true;
        }
    }
}