using System;
using System.Threading.Tasks;

namespace Skyiah.Web
{
    /// <summary>
    /// To do filtering right before executing an action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public abstract class BeforeAttribute : Attribute
    {
        readonly bool async;

        protected BeforeAttribute(bool async)
        {
            this.async = async;
        }

        public bool IsAsync => async;

        public abstract bool Do(WebContext wc);

        public abstract Task<bool> DoAsync(WebContext wc);
    }
}