using System;

namespace Greatbone.Core
{

    public enum UiMode
    {
        Link = 0x10,

        LinkDialog = 0x12,

        Button = 0x20,

        ButtonConfirm = 0x21,

        ButtonDialog = 0x22,

        AnchorDialog = 0x42,

        AnchorScript = 0x44,

    }

    /// 
    /// To specify user interaction related attributes and behaviors.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly UiMode mode;

        public UiAttribute(string label = null, UiMode mode = 0)
        {
            this.label = label;
            this.mode = mode;
        }

        public string Label => label;

        public UiMode Mode => mode;

        public int Limit { get; set; }

        public string Enable { get; set; }

        public bool IsZero => mode == 0;

        public bool IsLink => ((int)mode & 0x10) == 0x10;

        public bool IsButton => ((int)mode & 0x20) == 0x20;

        public bool IsAnchor => ((int)mode & 0x40) == 0x40;

        public bool HasConfirm => ((int)mode & 0x01) == 0x01;

        public bool HasDialog => ((int)mode & 0x02) == 0x02;

        public bool HasScript => ((int)mode & 0x04) == 0x04;
    }

}