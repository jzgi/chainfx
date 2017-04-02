using System;

namespace Greatbone.Core
{

    public enum UiMode
    {
        Link = 0x10,

        LinkDialog = 0x12,

        AnchorDialog = 0x22,

        Button = 0x40,

        ButtonConfirm = 0x41,

        ButtonDialog = 0x42,

        ButtonScript = 0x44,

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

        public int State { get; set; }

        public bool IsButton => ((int)Mode & 0x40) == 0x40;

        public bool IsAnchor => ((int)Mode & 0x20) == 0x20;

        public bool IsLink => ((int)Mode & 0x10) == 0x10;

        public bool HasConfirm => ((int)Mode & 0x01) == 0x01;

        public bool HasDialog => ((int)Mode & 0x02) == 0x02;

    }

}