using System;

namespace Greatbone.Core
{
    ///
    /// An exception handler filter.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class CatchAttribute : Attribute, IFilter
    {
        public Nodule Nodule { get; internal set; }

        protected internal abstract bool Catch(ActionContext ac, Exception ex);
    }
}