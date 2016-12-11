using System;

namespace Greatbone.Core
{

    ///
    /// An authorization check.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class CheckAttribute : Attribute
    {
        // if authenticated through cookie
        readonly bool cookie;

        public CheckAttribute(bool cookie = false)
        {
            this.cookie = cookie;
        }

        public virtual bool Check(WebActionContext wc)
        {
            return true;
        }

        public bool IsCookie => cookie;

    }

}