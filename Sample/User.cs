using Greatbone;

namespace Samp
{
    /// <summary>
    /// A user data object that is a principal.
    /// </summary>
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const byte
            ID = 1,
            PRIVACY = 2,
            LATER = 4;

        public const short
            OPR = 1,
            OPRMEM = 3,
            OPRMGR = 7;

        public static readonly Map<short, string> Oprs = new Map<short, string>
        {
            {OPR, "销售"},
            {OPRMEM, "内务"},
            {OPRMGR, "经理"},
        };

        internal int id;
        internal string name;
        internal string wx; // wexin openid
        internal string tel;
        internal string addr;
        public string credential;
        internal int score;
        internal int refid; // referee id
        internal string ctrid; // team
        internal string tmat; // team
        internal short tm;
        internal string vdrat; // vendor
        internal short vdr;
        internal string ctrat; // center
        internal short ctr;
        internal short plat; // platform

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) == ID)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(name), ref name);
            s.Get(nameof(wx), ref wx);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(addr), ref addr);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Get(nameof(credential), ref credential);
                s.Get(nameof(score), ref score);
                s.Get(nameof(refid), ref refid);
            }
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(tmat), ref tmat);
                s.Get(nameof(tm), ref tm);
                s.Get(nameof(vdrat), ref vdrat);
                s.Get(nameof(vdr), ref vdr);
                s.Get(nameof(ctrat), ref ctrat);
                s.Get(nameof(ctr), ref ctr);
                s.Get(nameof(plat), ref plat);
            }
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) == ID)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(name), name);
            s.Put(nameof(wx), wx);
            s.Put(nameof(tel), tel);
            s.Put(nameof(addr), addr);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Put(nameof(credential), credential);
                s.Put(nameof(score), score);
                s.Put(nameof(refid), refid);
            }
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(tmat), tmat);
                s.Put(nameof(tm), tm);
                s.Put(nameof(ctrat), ctrat);
                s.Put(nameof(ctr), ctr);
                s.Put(nameof(plat), plat);
            }
        }
    }
}