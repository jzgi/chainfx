using System;
using System.Threading.Tasks;

namespace Greatbone
{
    /// <summary>
    /// To implement authentication and authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AuthAttribute : Attribute
    {
        private int v;

        protected AuthAttribute(int v)
        {
            this.v = v;
        }

        public bool IsAsync => (v & 1) > 0;

        public abstract bool Authenticate(WebContext wc);

        public abstract Task<bool> AuthenticateAsync(WebContext wc);

        public abstract bool Authorize(WebContext wc);
    }
}