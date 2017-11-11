using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To specify interaction style.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class StyleAttribute : Attribute
    {
        readonly UiStyle style;

        readonly sbyte size;

        public StyleAttribute(UiStyle style, sbyte size = 2)
        {
            this.style = style;
            this.size = size;
        }

        public sbyte Size => size;

        public bool Circle { get; set; }

        public bool IsAnchor => ((int) style & 0x0100) == 0x0100;

        public bool IsButton => ((int) style & 0x0200) == 0x0200;

        public bool HasConfirm => ((int) style & 0x01) == 0x01;

        public bool HasPrompt => ((int) style & 0x02) == 0x02;

        public bool HasShow => ((int) style & 0x04) == 0x04;

        public bool HasOpen => ((int) style & 0x08) == 0x08;

        public bool HasScript => ((int) style & 0x10) == 0x10;

        public bool HasCrop => ((int) style & 0x20) == 0x20;
    }

    public enum UiStyle
    {
        Anchor = 0x0100,

        AnchorConfirm = 0x0101,

        /// <summary>
        /// To prompt with a dialog for gathering additional data to continue the location switch.
        /// </summary>
        AnchorPrompt = 0x0102,

        /// <summary>
        /// To show a dialog with the OK button for execution of an standalone activity
        /// </summary>
        AnchorShow = 0x0104,

        /// <summary>
        /// To open a free-style dialog without the OK button, where the action can be called a number of times.
        /// </summary>
        AnchorOpen = 0x0108,

        /// <summary>
        /// To execute a script that calls the action asynchronously.
        /// </summary>
        AnchorScript = 0x0110,

        AnchorCrop = 0x0120,

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
}