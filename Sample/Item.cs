using Greatbone.Core;

namespace Greatbone.Samp
{
    /// <summary>
    /// An item data object, that represents a product or service.
    /// </summary>
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        public const short PK = 1, LATER = 4;

        public const short OFF = 0, ON = 1, HOT = 2;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {OFF, "下架"},
            {ON, "上架"},
            {HOT, "推荐"},
        };

        internal string shopid;
        internal string name;
        internal string descr;
        internal string content;
        internal string unit;
        internal decimal price;
        internal short min;
        internal short step;
        internal short status;
        internal short stock; // remaining capacity

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & PK) == PK)
            {
                i.Get(nameof(shopid), ref shopid);
                i.Get(nameof(name), ref name);
            }
            i.Get(nameof(descr), ref descr);
            i.Get(nameof(content), ref content);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
            i.Get(nameof(min), ref min);
            i.Get(nameof(step), ref step);
            i.Get(nameof(status), ref status);
            i.Get(nameof(stock), ref stock);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & PK) == PK)
            {
                o.Put(nameof(shopid), shopid);
                o.Put(nameof(name), name);
            }
            o.Put(nameof(descr), descr);
            o.Put(nameof(content), content);
            o.Put(nameof(unit), unit);
            o.Put(nameof(price), price);
            o.Put(nameof(min), min);
            o.Put(nameof(step), step);
            o.Put(nameof(status), status);
            o.Put(nameof(stock), stock);
        }
    }
}