using System;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// To run before and/or after an action execution, for example to modify the request/response content.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class WorkAttribute : Attribute, IFilter
    {
        bool before;

        bool @async;

        public WorkAttribute(bool before, bool @async)
        {
            this.before = before;
            this.@async = @async;
        }

        public bool Before => before;

        public bool IsAsync => @async;

        public Nodule Nodule { get; internal set; }

        public abstract void Work(ActionContext ac);

        public abstract Task WorkAsync(ActionContext ac);
    }
}