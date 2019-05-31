using System;

namespace Greatbone.Web
{
    /// <summary>
    /// Thrown to indicate an failure of access check to a certain nodule. 
    /// </summary>
    public class AuthorizeException : Exception
    {
        readonly WebTarget _webTarget;

        readonly AuthorizeAttribute attribute;

        internal AuthorizeException(WebTarget webTarget, AuthorizeAttribute attribute = null, string msg = "authorize exception") : base(msg)
        {
            this._webTarget = webTarget;
            this.attribute = attribute;
        }

        /// <summary>
        /// The target work or action method.
        /// </summary>
        /// <seealso cref="WebWork"/>
        /// <seealso cref="WebAction"/>
        public WebTarget WebTarget => _webTarget;

        public AuthorizeAttribute Attribute => attribute;
    }
}