using System;

namespace Greatbone
{
    /// <summary>
    /// An access checking filter before the target nodule is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AccessAttribute : Attribute
    {
        /// <summary>
        /// An access check, such as against the current user, or other kinds of targets.
        /// </summary>
        /// <param name="wc"></param>
        /// <returns></returns>
        public abstract bool? Check(WebContext wc);
    }
}