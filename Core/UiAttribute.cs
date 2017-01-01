using System;

namespace Greatbone.Core
{
    /// 
    /// To specify user interface related attributes.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        public bool? FormPost { get; set; } = false;

        public string Label { get; set; } = null;

        public string Icon { get; set; } = null;

        public bool? DialogPost { get; set; } = false;

        public UiAttribute() { }
    }
}