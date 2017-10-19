using Greatbone.Core;

namespace Greatbone.Sample
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

        public const short OPR_ = 1, OPRMEM = 3, OPRMGR = 7;

        public static readonly Map<short, string> OPRS = new Map<short, string>
        {
            [OPR_] = "基本",
            [OPRMEM] = "成员",
            [OPRMGR] = "经理",
        };

        internal string wx; // wexin openid
        internal string name;
        internal string tel;
        internal string credential;
        internal string city; // 
        internal string area; // 
        internal string addr;
        internal short opr; // 
        internal short oprat; // operator at
        internal string oprname;
        internal bool adm; // adm

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & WX) == WX)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(city), ref city);
            i.Get(nameof(area), ref area);
            i.Get(nameof(addr), ref addr);
            if ((proj & LATER) == LATER)
            {
                i.Get(nameof(opr), ref opr);
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(oprname), ref oprname);
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
            o.Put(nameof(tel), tel);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(city), city);
            o.Put(nameof(area), area);
            o.Put(nameof(addr), addr);
            if ((proj & LATER) == LATER)
            {
                o.Put(nameof(opr), opr);
                o.Put(nameof(oprat), oprat);
                o.Put(nameof(oprname), oprname);
                o.Put(nameof(adm), adm);
            }
        }
    }
}