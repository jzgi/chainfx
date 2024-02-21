using System;

namespace ChainFX.Web
{
    /// <summary>
    /// To implement principal authorization of access to the target resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public abstract class AuthorizeAttribute : Attribute
    {
        /// <summary>
        /// The access type to check upon; A value of zero means any type, otherwise a specific access type.
        /// </summary>
        public short AccessTyp { get; protected set; }

        /// <summary>
        /// The role to check upon under certain access type.
        /// </summary>
        public short Role { get; protected set; }


        public abstract bool DoCheck(WebContext wc, out bool super);
    }
}