using System;
using System.Threading.Tasks;

namespace Greatbone
{
    /// <summary>
    /// To do filtering after executing an action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AfterAttribute : Attribute
    {
        readonly bool async;

        protected AfterAttribute(bool async)
        {
            this.async = async;
        }

        public bool IsAsync => async;

        public abstract bool Do(WebContext wc);

        public abstract Task<bool> DoAsync(WebContext wc);
    }
}