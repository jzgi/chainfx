using System;

namespace Greatbone.Core
{

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ButtonAttribute : Attribute
    {
        public bool IsGet { get; set; } = false;

        public string Icon { get; set; } = null;

        public int Dialog { get; set; } = 3;

        public ButtonAttribute() { }

    }

}