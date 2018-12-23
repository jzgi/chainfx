using System;
using Greatbone;

namespace Greatbone.Service
{
    /// <summary>
    /// To implement principal authorization of access to the target resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public abstract class AuthorizeAttribute : Attribute
    {
        public abstract bool Do(WebContext wc);
    }
}