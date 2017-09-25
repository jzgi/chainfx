using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// An item data object.
    /// </summary>
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        public const int SHOPID = 1;

        // status
        public static readonly Map<short, string> STATUS = new Map<short, string>
        {
            [0] = "下架",
            [1] = "上架仅展示",
            [2] = "上架可接单",
        };

        internal short shopid;
        internal string name;
        internal string descr;
        internal string unit;
        internal decimal price;
        internal short min;
        internal short step;
        internal short max;
        internal Part[] parts;
        internal short status;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            if ((proj & SHOPID) == SHOPID)
            {
                i.Get(nameof(shopid), ref shopid);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(descr), ref descr);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
            i.Get(nameof(min), ref min);
            i.Get(nameof(step), ref step);
            i.Get(nameof(max), ref max);
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & SHOPID) == SHOPID)
            {
                o.Put(nameof(shopid), shopid);
            }
            o.Put(nameof(name), name, "品名");
            o.Put(nameof(descr), descr, "描述");
            o.Put(nameof(unit), unit, "单位");
            o.Put(nameof(price), price, "单价");
            o.Put(nameof(min), min, "起订数量");
            o.Put(nameof(step), step, "递增因子");
            o.Put(nameof(max), max, "本批供应量");
            o.Put((string) nameof(status), status, "状态", STATUS);
        }
    }

    public struct Part
    {
        internal string name;

        internal short grams;
    }
}