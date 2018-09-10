using Greatbone;

namespace Samp
{
    /// <summary>
    /// A user data object that can act as a principal.
    /// </summary>
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const byte ID = 1, PRIVACY = 2;

        public const short TEAM = 1, TEAM_AID = 1, TEAM_MGMT = 15;

        public static readonly Map<short, string> Teamly = new Map<short, string>
        {
            {0, null},
            {TEAM_AID, "帮手"},
            {TEAM_MGMT, "团长"},
        };

        public const short HUB = 1, HUB_SCHEDULE = 0b0011, HUB_MGMT = 15;

        public static readonly Map<short, string> Hubly = new Map<short, string>
        {
            {HUB, "帮手"},
            {HUB_SCHEDULE, "调度"},
            {HUB_MGMT, "经理"},
        };

        internal int id;
        internal string wx; // wexin openid
        internal int refid; // referee id
        internal string name;
        internal string tel;
        public string credential;
        internal string addr;
        internal string teamat;
        internal short team;
        internal string shopat;
        internal short shop;
        internal short hub;
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
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(teamat), ref teamat);
            s.Get(nameof(team), ref team);
            s.Get(nameof(shopat), ref shopat);
            s.Get(nameof(shop), ref shop);
            s.Get(nameof(hub), ref hub);
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
            s.Put(nameof(addr), addr);
            s.Put(nameof(teamat), teamat);
            s.Put(nameof(team), team);
            s.Put(nameof(shopat), shopat);
            s.Put(nameof(shop), shop);
            s.Put(nameof(hub), hub);
            s.Put(nameof(created), created);
        }
    }
}