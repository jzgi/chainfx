using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public const short
            SUPER = 0x0010,
            ID = 0x0001,
            ICON = 0x0002;

        public static Opt<short> STATUS = new Opt<short>
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

        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(descr), ref descr);
            if ((proj & ICON) == ICON)
            {
                i.Get(nameof(icon), ref icon);
            }
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(distr), ref distr);
            i.Get(nameof(addr), ref addr);
            if ((proj & SUPER) == SUPER)
            {
                i.Get(nameof(lic), ref lic);
                i.Get(nameof(created), ref created);
                i.Get(nameof(mgrid), ref mgrid);
                i.Get(nameof(mgrwx), ref mgrwx);
                i.Get(nameof(mgr), ref mgr);
            }
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id, "编号");
            }
            o.Put(nameof(name), name, "名称");
            o.Put(nameof(descr), descr, "简语");
            if ((proj & ICON) == ICON)
            {
                o.Put(nameof(icon), icon, "照片");
            }
            o.Put(nameof(tel), tel, "电话");
            o.Put(nameof(city), city, "城市");
            o.Put(nameof(distr), distr, "区县");
            o.Put(nameof(addr), addr, "地址");
            if ((proj & SUPER) == SUPER)
            {
                o.Put(nameof(lic), lic, "工商登记");
                o.Put(nameof(created), created, "创建时间");
                o.Put(nameof(mgrid), mgrid);
                o.Put(nameof(mgrwx), mgrwx);
                o.Put(nameof(mgr), mgr);
            }
            o.Put(nameof(status), status, "状态", STATUS);
        }
    }
}