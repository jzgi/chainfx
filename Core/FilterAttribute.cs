using System;

namespace Greatbone.Core
{
    ///
    /// To alter relevant action context(s) through annotating on the action method(s) or its enclosing web folder.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class FilterAttribute : Attribute
    {
        public WebNodule Nodule { get; internal set; }

        public abstract void Before(WebActionContext ac);

        public abstract void After(WebActionContext ac);
    }
}