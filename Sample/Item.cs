using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// An item data object.
    /// </summary>
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        public const int UNMOD = 1;

        // status
        public static readonly Map<short, string> STATUS = new Map<short, string>
        {
            [0] = "下架",
            [1] = "上架",
        };

        internal short shopid;
        internal string name;
        internal string descr;
        internal string unit;
        internal decimal price;
        internal short min;
        internal short step;
        internal short max;
        internal short status;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            if ((proj & UNMOD) == UNMOD)
            {
                i.Get(nameof(shopid), ref shopid);
                i.Get(nameof(name), ref name);
            }
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
            if ((proj & UNMOD) == UNMOD)
            {
                o.Put(nameof(shopid), shopid);
                o.Put(nameof(name), name);
            }
            o.Put(nameof(descr), descr);
            o.Put(nameof(unit), unit);
            o.Put(nameof(price), price);
            o.Put(nameof(min), min);
            o.Put(nameof(step), step);
            o.Put(nameof(max), max);
            o.Put((string) nameof(status), status);
        }
    }

    public struct Part
    {
        internal string name;

        internal short grams;
    }
}