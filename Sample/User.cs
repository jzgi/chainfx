using Greatbone;

namespace Samp
{
    /// <summary>
    /// A user data object that can act as a principal.
    /// </summary>
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const byte ID = 1, PRIVACY = 2, LATER = 4;

        public const short TEAM = 1, TEAM_AID = 1, TEAM_MGMT = 15;

        public static readonly Map<short, string> Teamly = new Map<short, string>
        {
            {0, null},
            {1, "助手"},
            {15, "团长"},
        };

        public static readonly Map<short, string> Shoply = new Map<short, string>
        {
            {0, null},
            {1, "助手"},
            {15, "经理"},
        };

        public const short Reg = 1, RegScheduler = 0b0011, RegMgmt = 15;

        public static readonly Map<short, string> Hubly = new Map<short, string>
        {
            {Reg, "助手"},
            {RegScheduler, "调度"},
            {RegMgmt, "经理"},
        };

        internal int id;
        internal string name;
        internal string tel;
        public string credential;
        internal string wx; // wexin openid
        internal string addr;
        internal string teamat;
        internal short teamly;
        internal string shopat;
        internal short shoply;
        internal short regly;
        internal short created;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tel), ref tel);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Get(nameof(credential), ref credential);
            }
            s.Get(nameof(wx), ref wx);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(teamat), ref teamat);
            s.Get(nameof(teamly), ref teamly);
            s.Get(nameof(shopat), ref shopat);
            s.Get(nameof(shoply), ref shoply);
            s.Get(nameof(regly), ref regly);
            s.Get(nameof(created), ref created);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(tel), tel);
            if ((proj & PRIVACY) == PRIVACY)
            {
                s.Put(nameof(credential), credential);
            }
            s.Put(nameof(wx), wx);
            s.Put(nameof(addr), addr);
            s.Put(nameof(teamat), teamat);
            s.Put(nameof(teamly), teamly);
            s.Put(nameof(shopat), shopat);
            s.Put(nameof(shoply), shoply);
            s.Put(nameof(regly), regly);
            s.Put(nameof(created), created);
        }

        public bool IsIncomplete => name == null || tel == null | addr == null;
    }
}