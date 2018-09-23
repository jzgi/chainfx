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
        readonly int mod;

        protected FilterAttribute(int mod)
        {
            this.mod = mod;
        }

        public bool IsBefore => (mod & 1) > 0;

        public bool IsAfter => (mod & 2) > 0;

        public bool IsSync => (mod & 4) == 0;

        public bool IsAsync => (mod & 4) > 0;

        public abstract bool OnBefore(WebContext wc);

        public abstract Task<bool> OnBeforeAsync(WebContext wc);

        public abstract bool OnAfter(WebContext wc);

        public abstract Task<bool> OnAfterAsync(WebContext wc);
    }
}