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
            PK = 1,
            CREDENTIAL = 2,
            LATER = 4;

        public const short OPR = 1, OPRMEM = 3, OPRMGR = 7;

        public static readonly Map<short, string> Oprs = new Map<short, string>
        {
            {OPR, "基础"},
            {OPRMEM, "成员"},
            {OPRMGR, "经理"},
        };

        internal string wx; // wexin openid
        internal string name;
        internal string tel;
        internal string addr;
        internal string credential;
        internal int score;
        internal int refwx;
        internal string oprat;
        internal short opr;
        internal short adm;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Get(nameof(wx), ref wx);
            }
            s.Get(nameof(name), ref name);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(addr), ref addr);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                s.Get(nameof(credential), ref credential);
            }
            s.Get(nameof(score), ref score);
            s.Get(nameof(refwx), ref refwx);
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(opr), ref opr);
                s.Get(nameof(oprat), ref oprat);
                s.Get(nameof(adm), ref adm);
            }
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Put(nameof(wx), wx);
            }
            s.Put(nameof(name), name);
            s.Put(nameof(tel), tel);
            s.Put(nameof(addr), addr);
            if ((proj & CREDENTIAL) == CREDENTIAL)
            {
                s.Put(nameof(credential), credential);
            }
            s.Put(nameof(score), score);
            s.Put(nameof(refwx), refwx);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(opr), opr);
                s.Put(nameof(oprat), oprat);
                s.Put(nameof(adm), adm);
            }
        }
    }
}