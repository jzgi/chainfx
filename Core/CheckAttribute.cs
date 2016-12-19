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
        readonly bool cookied;

        public CheckAttribute(bool cookied = false)
        {
            this.cookied = cookied;
        }

        public WebControl Control { get; internal set; }

        public bool IsCookied => cookied;

        public virtual bool Check(WebActionContext ac) => true;
    }
}