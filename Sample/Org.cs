using Greatbone;

namespace Samp
{
    public class Org : IData, IGroupKeyable<string>
    {
        public static readonly Org Empty = new Org();

        public const byte ADM = 3, ID = 1, NORM = 4, LATER = 8;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "已停业"},
            {1, "休息中"},
            {2, "营业中"}
        };

        internal string id;
        internal string name;
        internal string descr;
        internal string addr;
        internal double x1;
        internal double y1;
        internal double x2;
        internal double y2;
        internal int mgrid;
        internal string mgrwx;
        internal string mgrname;
        internal string mgrtel;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) s.Get(nameof(id), ref id);
                s.Get(nameof(name), ref name);
                s.Get(nameof(descr), ref descr);
                s.Get(nameof(addr), ref addr);
                s.Get(nameof(x1), ref x1);
                s.Get(nameof(y1), ref y1);
                s.Get(nameof(x2), ref x2);
                s.Get(nameof(y2), ref y2);
            }
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
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) s.Put(nameof(id), id);
                s.Put(nameof(name), name);
                s.Put(nameof(descr), descr);
                s.Put(nameof(addr), addr);
                s.Put(nameof(x1), x1);
                s.Put(nameof(y1), y1);
                s.Put(nameof(x2), x2);
                s.Put(nameof(y2), y2);
            }
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

        public bool IsSupply => id.Length == 4 && id[2] >= 'E';

        public bool IsGroup => id.Length == 4 && id[2] < 'E';
    }
}