using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        public const short
            ID = 0x0001,
            ICON = 0x0002,
            LATE = 0x0010;


        public Opt<short> STATUS = new Opt<short>
        {
            [0] = "停业",
            [1] = "休假中",
            [2] = "营业中"
        };

        internal string id;
        internal string name;
        internal string descr;
        internal string icon;
        internal string tel;
        internal string city;
        internal string distr;
        internal string addr;
        internal double x;
        internal double y;
        internal string lic;
        internal DateTime created;
        internal int orders;
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
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            i.Get(nameof(lic), ref lic);
            i.Get(nameof(orders), ref orders);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id, label: "编号");
            }
            o.Put(nameof(name), name, label: "名称");
            o.Put(nameof(descr), descr, label: "简语");
            if ((proj & ICON) == ICON)
            {
                o.Put(nameof(icon), icon, label: "照片");
            }
            o.Put(nameof(tel), tel, label: "电话");
            o.Put(nameof(city), city, label: "城市");
            o.Put(nameof(distr), distr, label: "区县");
            o.Put(nameof(addr), addr, label: "地址");
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            o.Put(nameof(lic), lic, label: "工商登记");
            o.Put(nameof(orders), orders);
            o.Put(nameof(status), status, opt: STATUS, label: "状态");
        }
    }
}