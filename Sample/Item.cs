using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// An item data object, that represents a product or service.
    /// </summary>
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        public const byte PK = 1, LATER = 4, IMGG = 0x20;

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
        internal string unit;
        internal decimal price;
        internal short min;
        internal short step;
        internal short status;
        internal short stock; // remaining capacity

        internal bool imgg;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Get(nameof(shopid), ref shopid);
                s.Get(nameof(name), ref name);
            }
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(status), ref status);
            s.Get(nameof(stock), ref stock);
            if ((proj & IMGG) == IMGG)
            {
                s.Get(nameof(imgg), ref imgg);
            }
        }

        public void Write<R>(ISink<R> s, byte proj = 0x0f) where R : ISink<R>
        {
            if ((proj & PK) == PK)
            {
                s.Put(nameof(shopid), shopid);
                s.Put(nameof(name), name);
            }
            s.Put(nameof(descr), descr);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(status), status);
            s.Put(nameof(stock), stock);
        }
    }
}