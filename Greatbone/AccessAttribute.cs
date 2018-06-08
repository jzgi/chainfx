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
        /// An authorization check
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="prin"></param>
        /// <returns></returns>
        public abstract bool? Check(WebContext wc, IData prin);
    }
}