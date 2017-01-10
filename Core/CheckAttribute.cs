using System;

namespace Greatbone.Core
{
    ///
    /// An authorization check before the targetd component is invoked.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class CheckAttribute : Attribute
    {
        public WebControl Control { get; internal set; }

        public virtual bool Check(WebActionContext ac) => true;
    }
}