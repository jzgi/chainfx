using System;

namespace Greatbone.Core
{
    /// 
    /// To specify user interface related attributes.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        public string Label { get; set; } = null;

        public string Icon { get; set; } = null;

        public int Form { get; set; } = 0;

        public int Dialog { get; set; } = 0;
    }
}