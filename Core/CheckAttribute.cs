using System;

namespace Greatbone.Core
{
    ///
    /// An access check before the target nodule is invoked.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class CheckAttribute : Attribute
    {
        public Nodule Nodule { get; internal set; }

        public virtual bool Check(ActionContext ac) => true;
    }
}