using System;

namespace Greatbone.Core
{
    ///
    /// An access check before the target construct is invoked.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class AccessAttribute : Attribute
    {
        public Nodule Nodule { get; internal set; }

        public virtual bool Check(ActionContext ac) => true;
    }
}