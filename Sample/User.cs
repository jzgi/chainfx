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
            CTR = 1,
            CTR_SUPPLIER = 0b0011, // 3
            CTR_DELIVERER = 0b0101, // 5
            CTR_MANAGER = 0b0111; // 7

        public static readonly Map<short, string> Ctrs = new Map<short, string>
        {
            {CTR_SUPPLIER, "加工备货"},
            {CTR_DELIVERER, "派送"},
            {CTR_MANAGER, "经理"},
        };

        internal int id;
        internal string name;
        internal string wx; // wexin openid
        internal string tel;
        internal string grpat; // community team
        internal string addr;
        public string credential;
        internal int score;
        internal int refid; // referee id
        internal short ctr;
        internal short grp;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) == ID)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(name), ref name);
            s.Get(nameof(wx), ref wx);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(grpat), ref grpat);
            s.Get(nameof(addr), ref addr);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Get(nameof(credential), ref credential);
                s.Get(nameof(score), ref score);
                s.Get(nameof(refid), ref refid);
            }
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(ctr), ref ctr);
                s.Get(nameof(grp), ref grp);
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
            s.Put(nameof(grpat), grpat);
            s.Put(nameof(addr), addr);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Put(nameof(credential), credential);
                s.Put(nameof(score), score);
                s.Put(nameof(refid), refid);
            }
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(ctr), ctr);
                s.Put(nameof(grp), grp);
            }
        }
    }
}