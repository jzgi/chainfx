using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To bind notice functionality to action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class NoticeAttribute : Attribute
    {
        protected readonly short slot;

        protected NoticeAttribute(short slot)
        {
            this.slot = slot;
        }

        public short Slot => slot;

        public abstract int DoCheck(int noticeId, bool clear = false);
    }
}