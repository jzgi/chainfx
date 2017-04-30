using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A product or service.
    ///
    public class Item : IData
    {
        public const short
            SHOPID = 0x0800,
            ICON = 0x0200;


        public static readonly Item Empty = new Item();

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [0] = "架下",
            [1] = "缺货",
            [2] = "在售",
        };

        internal string shopid;
        internal string name;
        internal string descr;
        internal ArraySegment<byte> icon;
        internal string unit;
        internal decimal price; // current price
        internal int min; // minimal ordered
        internal int step;
        internal bool global;
        internal short status;

        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & SHOPID) == SHOPID)
            {
                i.Get(nameof(shopid), ref shopid);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(descr), ref descr);
            if ((proj & ICON) == ICON)
            {
                i.Get(nameof(icon), ref icon);
            }
            i.Get(nameof(price), ref price);
            i.Get(nameof(min), ref min);
            i.Get(nameof(step), ref step);
            i.Get(nameof(global), ref global);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & SHOPID) == SHOPID)
            {
                o.Put(nameof(shopid), shopid);
            }
            o.Put(nameof(name), name, label: "品名", max: 10, required: true);
            o.Put(nameof(unit), unit, label: "单位", max: 8, required: true);
            o.Put(nameof(descr), descr, label: "描述", max: 20, required: true);
            if ((proj & ICON) == ICON)
            {
                o.Put(nameof(icon), icon, label: "图片", size: "240,240", ratio: "1:1", required: true);
            }
            o.Put(nameof(price), price, label: "单价", required: true);
            o.Put(nameof(min), min, label: "起订");
            o.Put(nameof(step), step, label: "递增");
            o.Put(nameof(global), global, label: "不限同城");
            o.Put(nameof(status), status, label: "状态", opt: STATUS);
        }
    }
}