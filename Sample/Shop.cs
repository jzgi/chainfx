using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public const int
            ID = 0x0001,
            BASIC = 0x0002,
            BASIC_ICON = 0x006,
            ADMIN = 0x0020,
            ADMIN_WX = 0x0040;

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
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            i.Get(nameof(areas), ref areas);
            i.Get(nameof(mgrwx), ref mgrwx);
            i.Get(nameof(mgrtel), ref mgrtel);
            i.Get(nameof(mgrname), ref mgrname);
            i.Get(nameof(oprwx), ref oprwx);
            i.Get(nameof(oprtel), ref oprtel);
            i.Get(nameof(oprname), ref oprname);
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id, "编号");
            o.Put(nameof(name), name, "名称");
            o.Put(nameof(city), city, "城市");
            o.Put(nameof(addr), addr, "地址");
            o.Put(nameof(x), x, "X");
            o.Put(nameof(y), y, "Y");
            o.Put(nameof(areas), areas, "创建时间");
            o.Group("经理");
            o.Put(nameof(mgrwx), mgrwx);
            o.Put(nameof(mgrtel), mgrtel);
            o.Put(nameof(mgrname), mgrname);
            o.UnGroup();
            o.Group("值班");
            o.Put(nameof(oprwx), oprwx);
            o.Put(nameof(oprtel), oprtel);
            o.Put(nameof(oprname), oprname);
            o.UnGroup();
            o.Put(nameof(status), status, "状态", STATUS);
        }
    }
}