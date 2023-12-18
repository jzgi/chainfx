using System;

namespace ChainFX.Web
{
    /// <summary>
    /// To implement principal authorization of access to the target resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public abstract class AuthorizeAttribute : Attribute
    {
        // org role requirement (bitwise)
        private readonly short role;

        protected AuthorizeAttribute(short role)
        {
            this.role = role;
        }

        public short Role => role;

        public abstract bool DoCheck(WebContext wc, out bool super);
    }
}