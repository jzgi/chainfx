using System;

namespace Greatbone.Core
{

    ///
    /// Working on a directory or individual action, to manipulate the action context as well as the result returned from it.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class FilterAttribute : Attribute
    {
        // if apply to subdirectories
        bool sub;

        public FilterAttribute(bool sub)
        {
            this.sub = sub;
        }

        public virtual void Before(WebActionContext ac)
        {
        }

        public virtual void After(WebActionContext ac)
        {
        }
    }

}