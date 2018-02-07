using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An access check filter before the target nodule is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AccessAttribute : Attribute
    {
        public abstract bool Check(ActionContext ac);
    }
}