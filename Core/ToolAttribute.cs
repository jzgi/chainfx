using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To specify a user interface tool(set) that works in a particular pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ToolAttribute : Attribute
    {
        readonly int element; // ui element

        readonly int flow; // ui flow

        readonly sbyte size;

        public ToolAttribute(Modal modal, sbyte size = 2)
        {
            this.element = (int) modal & 0xff00;
            this.flow = (int) modal & 0x00ff;
            this.size = size;
        }

        public sbyte Size => size;

        public int Ordinals { get; set; }

        public bool Circle { get; set; }

        public bool IsAnchor => element == 0x0100;

        public bool IsButton => element == 0x0200;

        public bool HasConfirm => flow == 0x01;

        public bool HasPrompt => flow == 0x02;

        public bool HasShow => flow == 0x04;

        public bool HasOpen => flow == 0x08;

        public bool HasScript => flow == 0x10;

        public bool HasCrop => flow == 0x20;
    }
}