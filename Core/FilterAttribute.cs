using System;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// To run before and/or after an action execution.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class FilterAttribute : Attribute
    {
        readonly bool @async;

        public FilterAttribute(bool @async)
        {
            this.@async = @async;
        }

        public Nodule Nodule { get; internal set; }

        public bool IsAsync => @async;

        public virtual void Before(ActionContext ac) { }

        public virtual Task BeforeAsync(ActionContext ac) { return Task.CompletedTask; }

        public virtual void After(ActionContext ac) { }

        public virtual Task AfterAsync(ActionContext ac) { return Task.CompletedTask; }
    }
}