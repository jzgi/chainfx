using System;
using System.Threading.Tasks;
using Greatbone;

namespace Greatbone.Service
{
    /// <summary>
    /// To determine principal identity based on current web context. The interaction with user, however, is not included.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public abstract class AuthenticateAttribute : Attribute
    {
        readonly bool async;

        protected AuthenticateAttribute(bool async)
        {
            this.async = async;
        }

        public bool IsAsync => async;

        /// <summary>
        /// The synchronous version of authentication check.
        /// </summary>
        /// <remarks>The method only tries to establish principal identity within current web context, not responsible for any related user interaction.</remarks>
        /// <param name="wc">current web request/response context</param>
        /// <returns>true to indicate the prcoessing context can continue; false to indicate that a failure has occured that the current processing context shoud quit</returns>
        public abstract bool Do(WebContext wc);

        /// <summary>
        /// The asynchronous version of authentication check.
        /// </summary>
        /// <remarks>The method only tries to establish principal identity within current web context, not responsible for any related user interaction.</remarks>
        /// <param name="wc">current web request/response context</param>
        /// <returns>true to indicate the prcoessing context can continue; false to indicate that a failure has occured that the current processing context shoud quit</returns>
        public abstract Task<bool> DoAsync(WebContext wc);
    }
}