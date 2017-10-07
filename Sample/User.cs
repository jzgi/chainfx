using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A user data object that is a principal.
    /// </summary>
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const int
            WX = 1,
            CREDENTIAL = 2,
            PERM = 4;

        public const short OPR_ = 1, OPR_MEM = 3, OPR_MGR = 7;

        public static readonly Map<short, string> OPR = new Map<short, string>
        {
            [OPR_] = "基本",
            [OPR_MEM] = "成员",
            [OPR_MGR] = "经理",
        };

        internal string wx; // wexin openid
        internal string name;
        internal string tel;
        internal string credential;
        internal string city; // 
        internal string addr;
        internal short oprat; // operator at
        internal short opr; // 
        internal bool adm; // adm
        internal short status;


        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & WX) == WX)
            {
                i.Get(nameof(wx), ref wx);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(credential), ref credential);
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            if ((proj & PERM) == PERM)
            {
                i.Get(nameof(oprat), ref oprat);
                i.Get(nameof(opr), ref opr);
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
            o.Put(nameof(credential), credential);
            o.Put(nameof(city), city);
            o.Put(nameof(addr), addr);
            if ((proj & PERM) == PERM)
            {
                o.Put(nameof(oprat), oprat);
                o.Put(nameof(opr), opr);
                o.Put(nameof(adm), adm);
            }
        }
    }
}