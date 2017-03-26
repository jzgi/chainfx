using System;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// To run after an action execution.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AfterAttribute : Attribute
    {
        readonly bool @async;

        public AfterAttribute(bool @async)
        {
            this.@async = @async;
        }

        public Nodule Nodule { get; internal set; }

        public bool IsAsync => @async;

        public virtual void Do(ActionContext ac) { }

        public virtual Task DoAsync(ActionContext ac) { return Task.CompletedTask; }
    }
}