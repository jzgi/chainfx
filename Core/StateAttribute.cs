using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A constraint check annotation to determine the availability of a procedure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class StateAttribute : Attribute
    {
        public abstract bool Check(WebContext wc, object obj);
    }
}