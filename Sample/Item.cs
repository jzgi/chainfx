using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A product or service.
    ///
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [0] = "架下",
            [1] = "展示",
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
        internal short status;

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Prime())
            {
                i.Get(nameof(shopid), ref shopid);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(descr), ref descr);
            if (proj.Bin())
            {
                i.Get(nameof(icon), ref icon);
            }
            i.Get(nameof(price), ref price);
            i.Get(nameof(min), ref min);
            i.Get(nameof(step), ref step);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Power())
            {
                o.Put(nameof(shopid), shopid);
            }
            o.Put(nameof(name), name, label: "品名", max: 10, required: true);
            o.Put(nameof(unit), unit);
            o.Put(nameof(descr), descr);
            if (proj.Bin())
            {
                o.Put(nameof(icon), icon, label: "图片", size: "240,240", ratio: "1:1", required: true);
            }
            o.Put(nameof(price), price, required: true);
            o.Put(nameof(min), min);
            o.Put(nameof(step), step);
            o.Put(nameof(status), status, label: "状态", opt: STATUS);
        }
    }
}