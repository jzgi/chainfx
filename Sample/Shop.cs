using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public const short
            ID = 1,
            LATE = 4;

        public const short OFF = 1, ON = 2;

        public static Map<short, string> Status = new Map<short, string>
        {
            {0, string.Empty},
            {OFF, "休息中"},
            {ON, "营业中"}
        };

        internal string id;
        internal string name;
        internal string city;
        internal string addr;
        internal string schedule;
        internal string[] flags;
        internal string[] areas;
        internal string mgrwx;
        internal string mgrtel;
        internal string mgrname;
        internal string oprwx;
        internal string oprtel;
        internal string oprname;
        internal short status;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(schedule), ref schedule);
            i.Get(nameof(flags), ref flags);
            i.Get(nameof(areas), ref areas);
            if ((proj & LATE) == LATE)
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

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(city), city);
            o.Put(nameof(addr), addr);
            o.Put(nameof(schedule), schedule);
            o.Put(nameof(flags), flags);
            o.Put(nameof(areas), areas);
            if ((proj & LATE) == LATE)
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
    }
}