using System;

namespace ChainFx.Nodal
{
    /// <summary>
    /// To bind notice functionality to action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class TwinerUpdateAttribute : Attribute
    {
        protected readonly short slot;

        protected TwinerUpdateAttribute(short slot)
        {
            this.slot = slot;
        }

        public short Slot => slot;

        public abstract int DoCheck(int noticeId, bool clear = false);
    }
}