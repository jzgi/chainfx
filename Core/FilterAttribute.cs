using System;

namespace Greatbone.Core
{
    ///
    /// Working on a web folder or individual web action, to manipulate the action context as well as the result returned from it.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class FilterAttribute : Attribute
    {
        public WebControl Control { get; internal set; }

        public abstract void Before(WebActionContext ac);

        public abstract void After(WebActionContext ac);
    }
}