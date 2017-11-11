using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An access check filter before the target nodule is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class StateAttribute : Attribute
    {
        public abstract bool Check(object obj);
    }
}