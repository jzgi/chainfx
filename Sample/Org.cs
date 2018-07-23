using Greatbone;

namespace Samp
{
    public class Org : IData, IGroupKeyable<string>
    {
        public static readonly Org Empty = new Org();

        public const byte ID = 1, LATER = 2;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, null},
            {1, "休息中"},
            {2, "营业中"}
        };

        internal string id;
        internal string name;
        internal string addr;
        internal double x;
        internal double y;
        internal int mgrid;
        internal string mgrwx;
        internal string mgrname;
        internal string mgrtel;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(name), ref name);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(x), ref x);
            s.Get(nameof(y), ref y);
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(mgrid), ref mgrid);
                s.Get(nameof(mgrname), ref mgrname);
                s.Get(nameof(mgrwx), ref mgrwx);
                s.Get(nameof(mgrtel), ref mgrtel);
                s.Get(nameof(status), ref status);
            }
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(name), name);
            s.Put(nameof(addr), addr);
            s.Put(nameof(x), x);
            s.Put(nameof(y), y);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(mgrid), mgrid);
                s.Put(nameof(mgrname), mgrname);
                s.Put(nameof(mgrwx), mgrwx);
                s.Put(nameof(mgrtel), mgrtel);
                s.Put(nameof(status), status);
            }
        }

        public string Key => id;

        public override string ToString() => name;

        public bool GroupAs(string akey)
        {
            return TextUtility.Compare(id, akey, 2);
        }

        public bool IsCenter => id.Length == 2;

        public bool IsVendor => id.Length == 4 && id[2] >= 'E';

        public bool IsTeam => id.Length == 4 && id[2] < 'E';
    }
}