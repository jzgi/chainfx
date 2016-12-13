using System;

namespace Greatbone.Core
{

    ///
    /// Working on a directory or individual action, to manipulate the action context as well as the result returned from it.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AlterAttribute : Attribute
    {

        public abstract void Before(WebActionContext ac);

        public abstract void After(WebActionContext ac);

        ///
        /// If applied to sub-directories as well.
        ///
        public bool Global { get; set; } = false;
    }
}