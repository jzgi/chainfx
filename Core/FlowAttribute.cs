using System;

namespace Greatbone.Core
{
    /// 
    /// To specify workflow related attributes and behaviors.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class FlowAttribute : Attribute
    {
        public short[] State { get; set; }

        public short[] Status { get; set; }
    }
}