using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public const int
            ID = 1,
            INITIAL = 2,
            LATE = 4;

        public static Map<short, string> STATUS = new Map<short, string>
        {
            [0] = "停业",
            [1] = "休假中",
            [2] = "营业中"
        };

        internal short id;
        internal string name;
        internal string city;
        internal string addr;
        internal double x;
        internal double y;
        internal string[] areas;
        internal string mgrwx;
        internal string mgrtel;
        internal string mgrname;
        internal string oprwx;
        internal string oprtel;
        internal string oprname;
        internal short status;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            if ((proj & INITIAL) == INITIAL)
            {
                i.Get(nameof(name), ref name);
                i.Get(nameof(city), ref city);
                i.Get(nameof(addr), ref addr);
                i.Get(nameof(x), ref x);
                i.Get(nameof(y), ref y);
            }
            if ((proj & LATE) == LATE)
            {
                i.Get(nameof(areas), ref areas);
                i.Get(nameof(mgrwx), ref mgrwx);
                i.Get(nameof(mgrtel), ref mgrtel);
                i.Get(nameof(mgrname), ref mgrname);
                i.Get(nameof(oprwx), ref oprwx);
                i.Get(nameof(oprtel), ref oprtel);
                i.Get(nameof(oprname), ref oprname);
                i.Get(nameof(status), ref status);
            }
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id);
            }
            if ((proj & INITIAL) == INITIAL)
            {
                o.Put(nameof(name), name);
                o.Put(nameof(city), city);
                o.Put(nameof(addr), addr);
                o.Put(nameof(x), x);
                o.Put(nameof(y), y);
            }
            if ((proj & LATE) == LATE)
            {
                o.Put(nameof(areas), areas);
                o.Put(nameof(mgrwx), mgrwx);
                o.Put(nameof(mgrtel), mgrtel);
                o.Put(nameof(mgrname), mgrname);
                o.Put(nameof(oprwx), oprwx);
                o.Put(nameof(oprtel), oprtel);
                o.Put(nameof(oprname), oprname);
                o.Put(nameof(status), status);
            }
        }
    }
}