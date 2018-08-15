using Greatbone;

namespace Samp
{
    /// <summary>
    /// A user data object that is a principal.
    /// </summary>
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const byte ID = 1, PRIVACY = 2;

        public const short
            CTR = 1,
            CTR_DVR = 0b0011, // 3
            CTR_SPR = 0b0101, // 5
            CTR_MGR = 0b1111; // 15

        public static readonly Map<short, string> Ctrs = new Map<short, string>
        {
            {CTR_DVR, "派送"},
            {CTR_SPR, "供货"},
            {CTR_MGR, "经理"},
        };

        internal int id;
        internal string wx; // wexin openid
        internal int refid; // referee id
        internal string name;
        internal string tel;
        public string credential;
        internal string grpat; // community team
        internal string addr;
        internal short grp;
        internal short ctr;
        internal short created;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(wx), ref wx);
            s.Get(nameof(refid), ref refid);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tel), ref tel);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Get(nameof(credential), ref credential);
            }
            s.Get(nameof(grpat), ref grpat);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(grp), ref grp);
            s.Get(nameof(ctr), ref ctr);
            s.Get(nameof(created), ref created);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(wx), wx);
            s.Put(nameof(refid), refid);
            s.Put(nameof(name), name);
            s.Put(nameof(tel), tel);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Put(nameof(credential), credential);
            }
            s.Put(nameof(grpat), grpat);
            s.Put(nameof(addr), addr);
            s.Put(nameof(grp), grp);
            s.Put(nameof(ctr), ctr);
            s.Put(nameof(created), created);
        }
    }
}