using System;
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
            SUPER = 0x0020,
            SUPER_WX = 0x0040;

        public static Map<short, string> STATUS = new Map<short, string>
        {
            [0] = "停业",
            [1] = "休假中",
            [2] = "营业中"
        };

        internal string id;
        internal string name;
        internal string descr;
        internal ArraySegment<byte> icon;
        internal string tel;
        internal string city;
        internal string distr;
        internal string addr;

        internal string lic;
        internal DateTime created;
        internal string mgrid; // set by mgr
        internal string mgrwx;
        internal string mgr;
        internal short status;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            if ((proj & BASIC) == BASIC)
            {
                i.Get(nameof(name), ref name);
                i.Get(nameof(descr), ref descr);
                if ((proj & BASIC_ICON) == BASIC_ICON)
                {
                    i.Get(nameof(icon), ref icon);
                }
                i.Get(nameof(tel), ref tel);
                i.Get(nameof(city), ref city);
                i.Get(nameof(distr), ref distr);
                i.Get(nameof(addr), ref addr);
            }
            if ((proj & SUPER) == SUPER)
            {
                i.Get(nameof(lic), ref lic);
                i.Get(nameof(created), ref created);
                i.Get(nameof(mgrid), ref mgrid);
                if ((proj & SUPER_WX) == SUPER_WX)
                {
                    i.Get(nameof(mgrwx), ref mgrwx);
                }
                i.Get(nameof(mgr), ref mgr);
            }
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id, "编号");
            }
            if ((proj & BASIC) == BASIC)
            {
                o.Put(nameof(name), name, "名称");
                o.Put(nameof(descr), descr, "简语");
                if ((proj & BASIC_ICON) == BASIC_ICON)
                {
                    o.Put(nameof(icon), icon, "照片");
                }
                o.Put(nameof(tel), tel, "电话");
                o.Put(nameof(city), city, "城市");
                o.Put(nameof(distr), distr, "区县");
                o.Put(nameof(addr), addr, "地址");
            }
            if ((proj & SUPER) == SUPER)
            {
                o.Put(nameof(lic), lic, "工商登记");
                o.Put(nameof(created), created, "创建时间");
                o.Group("经理");
                o.Put(nameof(mgrid), mgrid);
                if ((proj & SUPER_WX) == SUPER_WX)
                {
                    o.Put(nameof(mgrwx), mgrwx);
                }
                o.Put(nameof(mgr), mgr);
                o.UnGroup();
            }
            o.Put(nameof(status), status, "状态", STATUS);
        }
    }
}