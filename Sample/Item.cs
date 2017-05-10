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
            SHOPID = 0x0003, // inclusive
            QTY = 0x0004,
            ICON = 0x0200;


        public static readonly Item Empty = new Item();

        // status
        public static readonly Opt<short> STATUS = new Opt<short>
        {
            [0] = "架下",
            [1] = "架上仅展示",
            [2] = "架上接单",
        };

        internal string shopid;
        internal string name;
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
            if ((proj & SHOPID) != 0)
            {
                i.Get(nameof(shopid), ref shopid);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(descr), ref descr);
            if ((proj & ICON) == ICON)
            {
                i.Get(nameof(icon), ref icon);
            }
            i.Get(nameof(unit), ref unit);
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
            if ((proj & SHOPID) != 0)
            {
                o.Put(nameof(shopid), shopid);
            }
            o.Put(nameof(name), name, label: "品名");
            o.Put(nameof(descr), descr, label: "描述");
            if ((proj & ICON) == ICON)
            {
                o.Put(nameof(icon), icon, label: "照片");
            }
            o.Put(nameof(unit), unit, label: "单位");
            o.Put(nameof(price), price, label: "单价");
            o.Put(nameof(min), min, label: "起订数量");
            o.Put(nameof(step), step, label: "递增因子");
            if ((proj & QTY) == QTY)
            {
                o.Put(nameof(qty), qty, label: "本批供应量");
            }
            o.Put(nameof(status), status, opt: STATUS, label: "状态");
        }
    }
}