using System;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// To run before an action execution.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class BeforeAttribute : Attribute
    {
        readonly bool @async;

        public BeforeAttribute(bool @async)
        {
            this.@async = @async;
        }

        public Nodule Nodule { get; internal set; }

        public bool IsAsync => @async;

        public virtual void Do(ActionContext ac) { }

        public virtual Task DoAsync(ActionContext ac) { return Task.CompletedTask; }
    }
}