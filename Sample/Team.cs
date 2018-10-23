using Greatbone;

namespace Samp
{
    /// <summary>
    /// A customer team data object.
    /// </summary>
    public class Team : IData, IKeyable<short>, IOrg
    {
        public static readonly Team Empty = new Team();

        public const byte ID = 1, MISC = 2;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "禁用"},
            {1, "启用"}
        };

        internal short id;
        internal string hubid;
        internal string name;
        internal string addr;
        internal double x;
        internal double y;
        internal string mgrname;
        internal string mgrtel;
        internal string mgrwx;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(hubid), ref hubid);
            s.Get(nameof(name), ref name);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(x), ref x);
            s.Get(nameof(y), ref y);
            if ((proj & MISC) > 0)
            {
                s.Get(nameof(mgrtel), ref mgrtel);
                s.Get(nameof(mgrname), ref mgrname);
                s.Get(nameof(mgrwx), ref mgrwx);
            }
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(hubid), hubid);
            s.Put(nameof(name), name);
            s.Put(nameof(addr), addr);
            s.Put(nameof(x), x);
            s.Put(nameof(y), y);
            if ((proj & MISC) > 0)
            {
                s.Put(nameof(mgrtel), mgrtel);
                s.Put(nameof(mgrname), mgrname);
                s.Put(nameof(mgrwx), mgrwx);
            }
            s.Put(nameof(status), status);
        }

        public short Key => id;

        public string Name => name;

        public override string ToString() => name;
    }
}