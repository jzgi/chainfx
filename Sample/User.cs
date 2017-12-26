using Greatbone.Core;

namespace Greatbone.Samp
{
    /// <summary>
    /// A user data object that is a principal.
    /// </summary>
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const short
            WX = 1,
            CREDENTIAL = 2,
            LATER = 4;

        public const short OPR = 1, OPRSTAFF = 3, OPRMGR = 7;

        public static readonly Map<short, string> Oprs = new Map<short, string>
        {
            {OPR, "基本"},
            {OPRSTAFF, "成员"},
            {OPRMGR, "经理"},
        };

        internal string wx; // wexin openid
        internal string name;
        internal string credential;
        internal string city; // 
        internal string addr;
        internal string tel;
        internal short opr; // 
        internal string oprat; // operator at
        internal bool adm; // adm

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & WX) == WX)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(name), ref name);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(tel), ref tel);
            if ((proj & LATER) == LATER)
            {
                i.Get(nameof(opr), ref opr);
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(adm), ref adm);
            }
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & WX) == WX)
            {
                o.Put(nameof(wx), wx);
            }
            o.Put(nameof(name), name);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(city), city);
            o.Put(nameof(addr), addr);
            o.Put(nameof(tel), tel);
            if ((proj & LATER) == LATER)
            {
                o.Put(nameof(opr), opr);
                o.Put(nameof(oprat), oprat);
                o.Put(nameof(adm), adm);
            }
        }
    }
}