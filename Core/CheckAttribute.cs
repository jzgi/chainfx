using System;

namespace Greatbone.Core
{

    ///
    /// Test if the request meets certain condition.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckAttribute : Attribute
    {
        // whether auth through cookie
        readonly bool cookie;

        public CheckAttribute(bool cookie = true)
        {
            this.cookie = cookie;
        }

        public virtual bool Test(WebContext wc)
        {
            return true;
        }

        public bool IsCookie => cookie;

    }

}