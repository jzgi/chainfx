using System;

namespace Greatbone.Core
{
    /// 
    /// To assure the validity of an action on UI and in the internal.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class StateAttribute : Attribute
    {
        readonly short[] specs;

        public StateAttribute(params short[] specs)
        {
            this.specs = specs;
        }
    }
}