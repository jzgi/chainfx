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

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) s.Get(nameof(id), ref id);
                s.Get(nameof(name), ref name);
                s.Get(nameof(city), ref city);
                s.Get(nameof(addr), ref addr);
                s.Get(nameof(x), ref x);
                s.Get(nameof(y), ref y);
            }
            if ((proj & NORM) == NORM)
            {
                s.Get(nameof(schedule), ref schedule);
                s.Get(nameof(delivery), ref delivery);
                s.Get(nameof(areas), ref areas);
                s.Get(nameof(min), ref min);
                s.Get(nameof(notch), ref notch);
                s.Get(nameof(off), ref off);
            }
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(mgrwx), ref mgrwx);
                s.Get(nameof(mgrtel), ref mgrtel);
                s.Get(nameof(mgrname), ref mgrname);
                s.Get(nameof(oprwx), ref oprwx);
                s.Get(nameof(oprtel), ref oprtel);
                s.Get(nameof(oprname), ref oprname);
                s.Get(nameof(status), ref status);
            }
        }

        public void Write<R>(ISink<R> s, byte proj = 0x0f) where R : ISink<R>
        {
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) s.Put(nameof(id), id);
                s.Put(nameof(name), name);
                s.Put(nameof(city), city);
                s.Put(nameof(addr), addr);
                s.Put(nameof(x), x);
                s.Put(nameof(y), y);
            }
            if ((proj & NORM) == NORM)
            {
                s.Put(nameof(schedule), schedule);
                s.Put(nameof(delivery), delivery);
                s.Put(nameof(areas), areas);
                s.Put(nameof(min), min);
                s.Put(nameof(notch), notch);
                s.Put(nameof(off), off);
            }
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(mgrwx), mgrwx);
                s.Put(nameof(mgrtel), mgrtel);
                s.Put(nameof(mgrname), mgrname);
                s.Put(nameof(oprwx), oprwx);
                s.Put(nameof(oprtel), oprtel);
                s.Put(nameof(oprname), oprname);
                s.Put(nameof(status), status);
            }
        }

        public string Key => id;
    }
}