using System;
using System.Threading.Tasks;

namespace Greatbone
{
    /// <summary>
    /// To implement principal authentication for the current web request/response context. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public abstract class AuthenticateAttribute : Attribute
    {
        readonly bool async;

        protected AuthenticateAttribute(bool async)
        {
            this.async = async;
        }

        public bool IsAsync => async;

        public abstract bool Do(WebContext wc);

        public abstract Task<bool> DoAsync(WebContext wc);
    }
}