using System;

namespace Greatbone.Core
{
    ///
    /// An authorization role check.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class RoleAttribute : Attribute
    {
        // if authenticated through cookie
        readonly bool cookied;

        public RoleAttribute(bool cookied = false)
        {
            this.cookied = cookied;
        }

        public WebControl Control { get; internal set; }

        public bool IsCookied => cookied;

        public virtual bool Check(WebActionContext ac) => true;
    }
}