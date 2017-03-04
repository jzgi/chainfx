using System;

namespace Greatbone.Core
{
    ///
    /// An access check before the target nodule is invoked.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AuthorizeAttribute : Attribute
    {
        public Nodule Nodule { get; internal set; }

        public virtual bool Check(ActionContext ac) => true;
    }
}