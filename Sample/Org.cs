using Greatbone;

namespace Samp
{
    /// <summary>
    /// An organizational unit such as a supplying shop or a customer group.
    /// </summary>
    public class Org : IData, IKeyable<string>
    {
        public static readonly Org Empty = new Org();

        public const byte ID = 1, LATER = 2;

        public static readonly Map<short, string> Typs = new Map<short, string>
        {
            {0, "团组"},
            {1, "作坊"},
        };

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "禁用"},
            {1, "休息"},
            {2, "工作"}
        };

        internal string id;
        internal short typ;
        internal string name;
        internal string tel;
        internal string addr;
        internal double x;
        internal double y;
        internal int mgrid;
        internal string mgrname;
        internal string mgrwx;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(x), ref x);
            s.Get(nameof(y), ref y);
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(mgrid), ref mgrid);
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
            s.Put(nameof(typ), typ);
            s.Put(nameof(name), name);
            s.Put(nameof(tel), tel);
            s.Put(nameof(addr), addr);
            s.Put(nameof(x), x);
            s.Put(nameof(y), y);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(mgrid), mgrid);
                s.Put(nameof(mgrname), mgrname);
                s.Put(nameof(mgrwx), mgrwx);
            }
            s.Put(nameof(status), status);
        }

        public string Key => id;

        public override string ToString() => name;

        public bool IsSupply => id.Length == 1;

        public bool IsGroup => id.Length == 3;
    }
}