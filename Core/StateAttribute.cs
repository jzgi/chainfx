using System;

namespace Greatbone.Core
{
    /// <summary>
    /// A constraint check annotation to determine the availability of an action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class StateAttribute : Attribute
    {
        public abstract bool Check(ActionContext ac, object model);
    }
}