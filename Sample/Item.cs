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
            SHOPID = 0x0001,
            QTY = 0x0002,
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
        internal string shopname;
        internal string descr;
        internal ArraySegment<byte> icon;
        internal string unit;
        internal decimal price; // current price
        internal short min; // minimal ordered
        internal short step;
        internal short qty;
        internal short status;

        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & SHOPID) == SHOPID)
            {
                i.Get(nameof(shopid), ref shopid);
                i.Get(nameof(shopname), ref shopname);
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
            if ((proj & QTY) == QTY)
            {
                i.Get(nameof(qty), ref qty);
            }
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & SHOPID) == SHOPID)
            {
                o.Put(nameof(shopid), shopid);
                o.Put(nameof(shopname), shopname);
            }
            o.Put(nameof(name), name, label: "品名");
            o.Put(nameof(unit), unit, label: "单位");
            o.Put(nameof(descr), descr, label: "描述");
            if ((proj & ICON) == ICON)
            {
                o.Put(nameof(icon), icon, label: "图片");
            }
            o.Put(nameof(price), price, label: "单价");
            o.Put(nameof(min), min, label: "起订");
            o.Put(nameof(step), step, label: "递增");
            if ((proj & QTY) == QTY)
            {
                o.Put(nameof(qty), qty, label: "数量");
            }
            o.Put(nameof(status), status, opt: STATUS, label: "状态");
        }
    }
}