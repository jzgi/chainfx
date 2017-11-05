using System;

namespace Greatbone.Core
{
    public enum UiStyle
    {
        A = 0x0100,

        /// <summary>
        /// To prompt with a dialog for gathering additional data to continue the location switch.
        /// </summary>
        APrompt = 0x0102,

        /// <summary>
        /// To show a dialog with the OK button for execution of an standalone activity
        /// </summary>
        AShow = 0x0104,

        /// <summary>
        /// To open a free-style dialog without the OK button, where the action can be called a number of times.
        /// </summary>
        AOpen = 0x0108,

        /// <summary>
        /// To execute a script that calls the action asynchronously.
        /// </summary>
        AScript = 0x0110,

        ACrop = 0x0120,

        Button = 0x0200,

        ButtonConfirm = 0x0201,

        /// <summary>
        /// To show a dialog for gathering additional data to continue the button submission for a post action.
        /// </summary>
        ButtonPrompt = 0x0202,

        ButtonShow = 0x0204,

        /// <summary>
        /// To open a free-style dialog, passing current form context
        /// </summary>
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

        public UiAttribute(string label = null, string tip = null)
        {
            this.label = label;
            this.tip = tip ?? label;
        }

        public string Label => label;

        public string Tip => tip;

        public UiStyle Style { get; set; }

        public sbyte Size { get; set; } = 2;

        /// <summary>
        /// The state bitwise value that enables the action. 
        /// </summary>
        public short State { get; set; }

        public bool Circle { get; set; }

        /// <summary>
        /// Is empohsized or not.
        /// </summary>
        public bool Em { get; set; }

        public bool IsA => ((int) Style & 0x0100) == 0x0100;

        public bool IsButton => ((int) Style & 0x0200) == 0x0200;

        public bool HasConfirm => ((int) Style & 0x01) == 0x01;

        public bool HasPrompt => ((int) Style & 0x02) == 0x02;

        public bool HasShow => ((int) Style & 0x04) == 0x04;

        public bool HasOpen => ((int) Style & 0x08) == 0x08;

        public bool HasScript => ((int) Style & 0x10) == 0x10;

        public bool HasCrop => ((int) Style & 0x20) == 0x20;
    }
}