using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To specify a user interface tool(set) that works in a particular pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ToolAttribute : Attribute
    {
        public const short
            MOD_SCRIPT = 0x0001,
            MOD_CONFIRM = 0x0002,
            MOD_CROP = 0x0004,
            MOD_PROMPT = 0x0008,
            MOD_SHOW = 0x0010,
            MOD_OPEN = 0x0020,
            MOD_ASTACK = 0x0040;

        public static readonly ToolAttribute BUTTON_CONFIRM = new ToolAttribute(Modal.ButtonConfirm);

        public static readonly ToolAttribute BUTTON_PICK_CONFIRM = new ToolAttribute(Modal.ButtonPickConfirm);

        public static readonly ToolAttribute BUTTON_SCRIPT = new ToolAttribute(Modal.ButtonScript);

        public static readonly ToolAttribute BUTTON_PICK_SCRIPT = new ToolAttribute(Modal.ButtonPickScript);

        readonly Modal modal;

        readonly int element; // ui element

        readonly int mode; // ui mode

        readonly int pick; // form value pick

        readonly short status;

        readonly short state;

        // only for crop
        readonly short size;

        // only for crop
        readonly short subs;

        public ToolAttribute(Modal modal, short status = 0, short state = 0, short size = 1, short subs = 0)
        {
            this.modal = modal;
            this.element = (int) modal & 0xf000;
            this.mode = (int) modal & 0x00ff;
            this.pick = (int) modal & 0x0f00;
            this.status = status;
            this.state = state;
            this.size = size;
            this.subs = subs;
        }

        public Modal Modal => modal;

        public int Mode => mode;

        public short Size => size;

        public short Subs => subs;

        public bool IsAnchor => element == 0x1000 && mode == 0;

        public bool IsAnchorTag => element == 0x1000;

        public bool IsButtonTag => element == 0x2000;

        public bool IsPost => HasConfirm || HasPrompt;

        public bool MustPick => pick == 0x0100;

        public bool HasScript => mode == MOD_SCRIPT;

        public bool HasConfirm => mode == MOD_CONFIRM;

        public bool HasCrop => mode == MOD_CROP;

        public bool HasAnyDialog => mode >= MOD_PROMPT;

        public bool HasPrompt => mode == MOD_PROMPT;

        public bool HasShow => mode == MOD_SHOW;

        public bool HasOpen => mode == MOD_OPEN;

        public bool HasAStack => mode == MOD_ASTACK;

        public short Status => status;

        public short State => state;

        internal bool Meets(short stu, short sta)
        {
            bool okstu = (status == 0) || ((status & stu) != 0);
            if (okstu)
            {
                return (state == 0) || ((state & sta) != 0);
            }
            return false;
        }
    }
}