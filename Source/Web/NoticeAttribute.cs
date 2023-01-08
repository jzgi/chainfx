using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To bind notice functionality to action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class NoticeAttribute : Attribute
    {
        readonly short typ;

        protected NoticeAttribute(short typ)
        {
            this.typ = typ;
        }

        public short Typ => typ;

        public abstract int DoCheck();

        public abstract void DoClear(WebContext wc);
    }
}