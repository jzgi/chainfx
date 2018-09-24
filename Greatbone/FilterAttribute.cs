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

        public bool Before => (mod & 0b01) > 0;

        public bool BeforeAsync => (mod & 0b11) > 0;

        public bool After => (mod & 0b0100) > 0;

        public bool AfterAsync => (mod & 0b1100) > 0;

        public abstract bool OnBefore(WebContext wc);

        public abstract Task<bool> OnBeforeAsync(WebContext wc);

        public abstract bool OnAfter(WebContext wc);

        public abstract Task<bool> OnAfterAsync(WebContext wc);
    }
}