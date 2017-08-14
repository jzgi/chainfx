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

        public const ushort BASIC = 0x0001, BASIC_SHOPID = 0x0003, BASIC_ICON = 0x0005;

        // status
        public static readonly Map<short, string> STATUS = new Map<short, string>
        {
            [0] = "下架",
            [1] = "上架仅展示",
            [2] = "上架可接单",
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

        public void Read(IDataInput i, ushort proj = 0x00ff)
        {
            if ((proj & BASIC) == BASIC)
            {
                if ((proj & BASIC_SHOPID) == BASIC_SHOPID)
                {
                    i.Get(nameof(shopid), ref shopid);
                }
                i.Get(nameof(name), ref name);
                i.Get(nameof(descr), ref descr);
                if ((proj & BASIC_ICON) == BASIC_ICON)
                {
                    i.Get(nameof(icon), ref icon);
                }
                i.Get(nameof(unit), ref unit);
                i.Get(nameof(price), ref price);
                i.Get(nameof(min), ref min);
                i.Get(nameof(step), ref step);
                i.Get(nameof(qty), ref qty);
            }
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, ushort proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & BASIC) == BASIC)
            {
                if ((proj & BASIC_SHOPID) == BASIC_SHOPID)
                {
                    o.Put(nameof(shopid), shopid);
                }
                o.Put(nameof(name), name, label: "品名");
                o.Put(nameof(descr), descr, label: "描述");
                if ((proj & BASIC_ICON) == BASIC_ICON)
                {
                    o.Put(nameof(icon), icon, label: "照片");
                }
                o.Put(nameof(unit), unit, label: "单位");
                o.Put(nameof(price), price, label: "单价");
                o.Put(nameof(min), min, label: "起订数量");
                o.Put(nameof(step), step, label: "递增因子");
                o.Put(nameof(qty), qty, label: "本批供应量");
            }
            o.Put((string) nameof(status), status, (string) "状态", STATUS);
        }
    }
}