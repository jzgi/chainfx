using System;

namespace Greatbone
{
    /// <summary>
    /// To specify a user interface tool(set) that works in a particular pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ToolAttribute : Attribute
    {
        readonly int element; // ui element

        readonly int mode; // ui mode

        readonly int pick; // form value pick

        readonly Style style; // trigger style

        readonly byte size;

        readonly bool auth;

        public ToolAttribute(Modal modal, Style style = Style.Default, byte size = 1, bool auth = false)
        {
            this.element = (int) modal & 0xf000;
            this.mode = (int) modal & 0x00ff;
            this.pick = (int) modal & 0x0f00;
            this.size = size;
            this.style = style;
            this.auth = auth;
        }

        public byte Size => size;

        public Style Style => style;

        public bool Auth => auth;

        public int Ordinals { get; set; }

        public bool IsAnchorTag => element == 0x1000;

        public bool IsButtonTag => element == 0x2000;

        public bool MustPick => pick == 0x0100;

        public bool HasConfirm => mode == 0x01;

        public bool HasPrompt => mode == 0x02;

        public bool HasShow => mode == 0x04;

        public bool HasOpen => mode == 0x08;

        public bool HasScript => mode == 0x10;

        public bool HasCrop => mode == 0x20;
    }
}