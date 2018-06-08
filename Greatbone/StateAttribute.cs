using System;

namespace Greatbone
{
    /// <summary>
    /// A constraint check to determine the availability of a procedure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class StateAttribute : Attribute
    {
        public abstract bool Check(WebContext wc, object[] stack, int level);
    }
}