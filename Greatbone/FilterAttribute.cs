using System;
using System.Threading.Tasks;

namespace Greatbone
{
    /// <summary>
    /// To filter before executing a procedure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class FilterAttribute : Attribute
    {
        private int v;

        protected FilterAttribute(int v)
        {
            this.v = v;
        }

        public bool IsBefore => (v & 1) > 0;

        public bool IsAsync => (v & 0b10) > 0;

        public abstract bool OnBefore(WebContext wc);

        public abstract Task<bool> OnBeforeAsync(WebContext wc);

        public abstract bool OnAfter(WebContext wc);

        public abstract Task<bool> OnAfterAsync(WebContext wc);
    }
}