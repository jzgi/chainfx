using System;

namespace Greatbone.Core
{
    ///
    /// To run before and/or after an action execution, for example to modify the request/response content.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class WorkAttribute : Attribute, IFilter
    {
        bool before;

        public WorkAttribute(bool before)
        {
            this.before = before;
        }

        public bool Before => before;

        public Nodule Nodule { get; internal set; }

        public abstract void Work(ActionContext ac);
    }
}