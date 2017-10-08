using System;

namespace Greatbone.Core
{
    public enum Modal
    {
        A = 0x0100,

        AConfirm = 0x0101,

        /// To prompt with a dialog for gathering additional data to continue the location switch.
        APrompt = 0x0102,

        /// To show a dialog with the OK button for execution of standalone activity
        AShow = 0x0104,

        /// To open a free-style dialog
        AOpen = 0x0108,

        AScript = 0x0110,

        ACrop = 0x0120,

        Button = 0x0200,

        ButtonConfirm = 0x0201,

        /// To show a dialog for gathering additional data to continue the button submission.
        ButtonPrompt = 0x0202,

        ButtonShow = 0x0204,

        /// To open a free-style dialog, passing current form context
        ButtonOpen = 0x0208,

        ButtonScript = 0x0210,

        ButtonCrop = 0x0220,
    }

    /// 
    /// To specify user interaction related attributes and behaviors.
    ///
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class UiAttribute : Attribute
    {
        readonly string label;

        readonly string tip;

        readonly string icon;

        public UiAttribute(string label = null, string tip = null, string icon = null)
        {
            this.label = label;
            this.tip = tip ?? label;
            this.icon = icon;
        }

        public string Label => label;

        public string Tip => tip;

        public string Icon => icon;

        public Modal Modal { get; set; }

        /// <summary>
        /// The state bitwise value that enables the action. 
        /// </summary>
        public int State { get; set; }

        public bool Covers(int v)
        {
            return State == 0 || (State & v) == v;
        }

        public short X { get; set; } = 120;

        public short Y { get; set; } = 120;

        public bool Circle { get; set; } = false;

        /// <summary>
        /// Is empohsized or not.
        /// </summary>
        public bool Em { get; set; } = false;

        public bool IsA => ((int) Modal & 0x0100) == 0x0100;

        public bool IsButton => ((int) Modal & 0x0200) == 0x0200;

        public bool HasConfirm => ((int) Modal & 0x01) == 0x01;

        public bool HasPrompt => ((int) Modal & 0x02) == 0x02;

        public bool HasShow => ((int) Modal & 0x04) == 0x04;

        public bool HasOpen => ((int) Modal & 0x08) == 0x08;

        public bool HasScript => ((int) Modal & 0x10) == 0x10;

        public bool HasCrop => ((int) Modal & 0x20) == 0x20;
    }
}