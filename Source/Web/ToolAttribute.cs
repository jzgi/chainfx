using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To specify a user interface tool(set) that works in a particular pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ToolAttribute : Attribute
    {
        public static readonly ToolAttribute BUTTON_CONFIRM = new ToolAttribute(Modal.ButtonConfirm);

        public static readonly ToolAttribute BUTTON_PICK_CONFIRM = new ToolAttribute(Modal.ButtonPickConfirm);

        public static readonly ToolAttribute BUTTON_SCRIPT = new ToolAttribute(Modal.ButtonScript);

        public static readonly ToolAttribute BUTTON_PICK_SCRIPT = new ToolAttribute(Modal.ButtonPickScript);

        readonly Modal modal;

        readonly int element; // ui element

        readonly int mode; // ui mode

        readonly int pick; // form value pick

        // only for crop
        readonly short siz;

        // only for crop
        readonly short subs;

        public ToolAttribute(Modal modal, short siz = 1, short subs = 0)
        {
            this.modal = modal;
            this.element = (int) modal & 0xf000;
            this.mode = (int) modal & 0x00ff;
            this.pick = (int) modal & 0x0300;
            this.siz = siz;
            this.subs = subs;
        }

        public Modal Modal => modal;

        public int Mode => mode;

        public short Siz => siz;

        public short Subs => subs;

        public bool IsAnchorTag => element == 0x1000;

        public bool IsButtonTag => element == 0x2000;

        public bool IsPost => HasConfirm || HasPrompt;

        public bool MustPick => pick == 0x0100;

        public bool HasConfirm => mode == 0x0001;

        public bool HasPrompt => mode == 0x0002;

        public bool HasOpen => mode == 0x0004;

        public bool HasTell => mode == 0x0008;

        public bool HasStack => mode == 0x0010;

        public bool HasCrop => mode == 0x0020;

        public bool HasScript => mode == 0x0040;
    }
}