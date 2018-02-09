using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData, IMappable<string>
    {
        public static readonly Shop Empty = new Shop();

        public const byte ADM = 3, ID = 1, NORM = 4, LATER = 8;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "停业中"},
            {1, "休息中"},
            {2, "营业中"}
        };

        internal string id;
        internal string name;
        internal string city;
        internal string addr;
        internal double x;
        internal double y;
        internal string schedule;
        internal string delivery;
        internal string[] areas;
        internal decimal min;
        internal decimal notch;
        internal decimal off;
        internal string mgrwx;
        internal string mgrtel;
        internal string mgrname;
        internal string oprwx;
        internal string oprtel;
        internal string oprname;
        internal short status;

        public void Read(IDataInput i, byte proj = 0x0f)
        {
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) i.Get(nameof(id), ref id);
                i.Get(nameof(name), ref name);
                i.Get(nameof(city), ref city);
                i.Get(nameof(addr), ref addr);
                i.Get(nameof(x), ref x);
                i.Get(nameof(y), ref y);
            }
            if ((proj & NORM) == NORM)
            {
                i.Get(nameof(schedule), ref schedule);
                i.Get(nameof(delivery), ref delivery);
                i.Get(nameof(areas), ref areas);
                i.Get(nameof(min), ref min);
                i.Get(nameof(notch), ref notch);
                i.Get(nameof(off), ref off);
            }
            if ((proj & LATER) == LATER)
            {
                i.Get(nameof(mgrwx), ref mgrwx);
                i.Get(nameof(mgrtel), ref mgrtel);
                i.Get(nameof(mgrname), ref mgrname);
                i.Get(nameof(oprwx), ref oprwx);
                i.Get(nameof(oprtel), ref oprtel);
                i.Get(nameof(oprname), ref oprname);
                i.Get(nameof(status), ref status);
            }
        }

        public void Write<R>(IDataOutput<R> o, byte proj = 0x0f) where R : IDataOutput<R>
        {
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) o.Put(nameof(id), id);
                o.Put(nameof(name), name);
                o.Put(nameof(city), city);
                o.Put(nameof(addr), addr);
                o.Put(nameof(x), x);
                o.Put(nameof(y), y);
            }
            if ((proj & NORM) == NORM)
            {
                o.Put(nameof(schedule), schedule);
                o.Put(nameof(delivery), delivery);
                o.Put(nameof(areas), areas);
                o.Put(nameof(min), min);
                o.Put(nameof(notch), notch);
                o.Put(nameof(off), off);
            }
            if ((proj & LATER) == LATER)
            {
                o.Put(nameof(mgrwx), mgrwx);
                o.Put(nameof(mgrtel), mgrtel);
                o.Put(nameof(mgrname), mgrname);
                o.Put(nameof(oprwx), oprwx);
                o.Put(nameof(oprtel), oprtel);
                o.Put(nameof(oprname), oprname);
                o.Put(nameof(status), status);
            }
        }

        public string Key => id;
    }
}