using Greatbone;

namespace Core
{
    /// <summary>
    /// A user data object that is a principal.
    /// </summary>
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const byte
            WX = 1,
            CREDENTIAL = 2,
            LATER = 4;

        public const short OPR = 1, OPRSTAFF = 3, OPRMGR = 7;

        public static readonly Map<short, string> Oprs = new Map<short, string>
        {
            {OPR, "客串"},
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
        internal int score;
        internal string refwx;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & WX) == WX)
            {
                s.Get(nameof(wx), ref wx);
            }
            s.Get(nameof(name), ref name);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                s.Get(nameof(credential), ref credential);
            }
            s.Get(nameof(city), ref city);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(score), ref score);
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(opr), ref opr);
                s.Get(nameof(oprat), ref oprat);
                s.Get(nameof(adm), ref adm);
            }
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & WX) == WX)
            {
                s.Put(nameof(wx), wx);
            }
            s.Put(nameof(name), name);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                s.Put(nameof(credential), credential);
            }
            s.Put(nameof(city), city);
            s.Put(nameof(addr), addr);
            s.Put(nameof(tel), tel);
            s.Put(nameof(score), score);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(opr), opr);
                s.Put(nameof(oprat), oprat);
                s.Put(nameof(adm), adm);
            }
        }
    }
}