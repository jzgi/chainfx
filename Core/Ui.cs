using System;

namespace Greatbone.Core
{

    public enum UiMode : int
    {
        Link = 0x10,

        LinkDialog = 0x12,

        AnchorDialog = 0x22,

        AnchorScript = 0x24,

        AnchorCrop = 0x28,

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
        readonly string label;

        readonly UiMode mode;

        public UiAttribute(string label = null, UiMode mode = UiMode.Button)
        {
            this.label = label;
            this.mode = mode;
        }

        public string Label => label;

        public UiMode Mode => mode;

        public short Width { get; set; } = 120;

        public short Height { get; set; } = 120;

        public bool Circle { get; set; } = false;

        public bool Alert { get; set; } = false;

        public bool IsLink => ((int)mode & 0x10) == 0x10;

        public bool IsAnchor => ((int)mode & 0x20) == 0x20;

        public bool IsButton => ((int)mode & 0x40) == 0x40;

        public bool HasConfirm => ((int)mode & 0x01) == 0x01;

        public bool HasDialog => ((int)mode & 0x02) == 0x02;

        public bool HasScript => ((int)mode & 0x04) == 0x04;

        public bool HasCrop => ((int)mode & 0x08) == 0x08;
    }

}