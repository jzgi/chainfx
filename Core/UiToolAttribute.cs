using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To specify a single UI element or a combination of multiple UI elements that work in a particular scheme.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class UiToolAttribute : Attribute
    {
        readonly UiMode mode;

        readonly int element; // ui element

        readonly int flow; // ui flow

        readonly sbyte size;

        public UiToolAttribute(UiMode mode, sbyte size = 2)
        {
            this.mode = mode;
            this.element = (int) mode & 0xff00;
            this.flow = (int) mode & 0x00ff;
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