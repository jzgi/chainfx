using System;

namespace Greatbone
{
    /// <summary>
    /// An authorization check filter before the target nodule is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AuthorizeAttribute : Attribute
    {
        public abstract bool? Check(WebContext wc, IData prin);
    }
}