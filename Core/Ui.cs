using System;

namespace Greatbone.Core
{
    public enum UiMode : int
    {
        Anchor = 0x0100,

        /// To prompt with a dialog for gathering additional data to continue the location switch.
        AnchorPrompt = 0x0102,

        /// To show a dialog with the OK button for execution of standalone activity
        AnchorShow = 0x0104,

        /// To open a free-style dialog
        AnchorOpen = 0x0108,

        AnchorScript = 0x0110,

        AnchorCrop = 0x0120,

        Button = 0x0200,

        ButtonConfirm = 0x0201,

        /// To show a dialog for gathering additional data to continue the button submission.
        ButtonPrompt = 0x0202,

        ButtonShow = 0x0204,

        /// To open a free-style dialog, passing current form context
        ButtonOpen = 0x0208,

        ButtonScript = 0x0210,
    }

    /// 
    /// To specify user interaction related attributes and behaviors.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly string tip;

        public UiAttribute(string label = null, string tip = null)
        {
            this.label = label;
            this.tip = tip;
        }

        public string Label => label;

        public string Tip => tip;

        public string TipOrLabel => tip ?? label;

        public UiMode Mode { get; set; } = UiMode.Button;

        public short Width { get; set; } = 120;

        public short Height { get; set; } = 120;

        public bool Circle { get; set; } = false;

        public bool Bold { get; set; } = false;

        public bool IsAnchor => ((int)Mode & 0x0100) == 0x0100;

        public bool IsButton => ((int)Mode & 0x0200) == 0x0200;

        public bool HasConfirm => ((int)Mode & 0x01) == 0x01;

        public bool HasPrompt => ((int)Mode & 0x02) == 0x02;

        public bool HasShow => ((int)Mode & 0x04) == 0x04;

        public bool HasOpen => ((int)Mode & 0x08) == 0x08;

        public bool HasScript => ((int)Mode & 0x10) == 0x10;

        public bool HasCrop => ((int)Mode & 0x20) == 0x20;
    }
}