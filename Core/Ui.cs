using System;

namespace Greatbone.Core
{

    public enum UiMode
    {
        Script = 22,

        Link = 0,

        LinkDialog = 1,

        AnchorDialog = 10,

        Button = 20,

        ButtonConfirm = 21,

        ButtonDialog = 22,
    }

    /// 
    /// To specify user interaction related attributes and behaviors.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        public UiAttribute() { }

        public UiAttribute(string label, string icon = null, UiMode mode = 0)
        {
            Label = label;
            Icon = icon;
            Mode = mode;
        }

        public string Label { get; set; }

        public string Icon { get; set; }

        public UiMode Mode { get; set; }

        public int Limit { get; set; }
    }

}