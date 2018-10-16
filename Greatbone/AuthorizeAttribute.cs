using System;

namespace Greatbone
{
    /// <summary>
    /// To implement principal authorization of access to the target resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public abstract class AuthorizeAttribute : Attribute
    {
        public abstract bool Do(WebContext wc);
    }
}