using System;

namespace Greatbone.Core
{
    /// 
    /// To specify user interface related attributes.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        public bool IsGet { get; set; } = false;

        public string Icon { get; set; } = null;

        public int Dialog { get; set; } = 3;

        public UiAttribute() { }
    }
}