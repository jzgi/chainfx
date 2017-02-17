using System;

namespace Greatbone.Core
{
    /// 
    /// To specify user interaction related attributes and behaviors.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        public UiAttribute() { }

        public UiAttribute(string label, string icon = null)
        {
            Label = label;
            Icon = icon;
        }

        public string Label { get; set; } = null;

        public string Icon { get; set; } = null;

        ///
        /// <remarks>
        /// 0 No Dialog
        /// 1 Small Dialog
        /// 2 Large Dialog
        /// 3 Full Standalone
        /// </remarks>
        public int Dialog { get; set; } = 1;
    }
}