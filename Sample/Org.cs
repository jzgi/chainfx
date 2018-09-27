using Greatbone;

namespace Samp
{
    /// <summary>
    /// An organizational unit such as a work shop or a customer team.
    /// </summary>
    public class Org : IData, IGroupKeyable<(string, short)>
    {
        public static readonly Org Empty = new Org();

        public const byte PK = 1, LATER = 2;

        public static readonly Map<short, string> Typs = new Map<short, string>
        {
            {0, "团组"},
            {1, "产供"},
        };

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "禁用"},
            {1, "休息"},
            {2, "工作"}
        };

        internal short id;
        internal string hubid;
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
            if ((proj & PK) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(hubid), ref hubid);
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
            if ((proj & PK) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(hubid), hubid);
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

        public (string, short) Key => (hubid, id);

        public bool GroupAs((string, short) akey)
        {
            return hubid == akey.Item1;
        }

        public override string ToString() => name;
    }
}