using System;

namespace Greatbone.Core
{
    ///
    /// An authorization role check.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class CheckAttribute : Attribute
    {
        public CheckAttribute()
        {
        }

        public WebControl Control { get; internal set; }

        public virtual bool Check(WebActionContext ac) => true;
    }
}