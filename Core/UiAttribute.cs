using System;

namespace Greatbone.Core
{
    /// 
    /// To specify user interaction related attributes and behaviors.
    ///
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        public string Label { get; set; } = null;

        public string Icon { get; set; } = null;

        public int Form { get; set; } = 0;

        public int Dialog { get; set; } = 0;

        /// To activate/deactivate the action according to state specifications.
        ///
        public int State { get; set; }

    }

}