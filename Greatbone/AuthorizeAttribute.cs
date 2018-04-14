using System;

namespace Greatbone
{
    /// <summary>
    /// An authorization check filter before the target nodule is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AuthorizeAttribute : Attribute
    {
        public virtual bool Ready(IData prin) => true;

        public abstract bool Allowed(IData prin, WebContext wc);
    }
}