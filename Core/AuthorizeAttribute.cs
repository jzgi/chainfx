using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An access check filter before the target nodule is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class AuthorizeAttribute : Attribute
    {
        public Nodule Nodule { get; internal set; }

        public abstract bool Check(ActionContext ac);
    }
}