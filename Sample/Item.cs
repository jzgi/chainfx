using Greatbone;

namespace Samp
{
    /// <summary>
    /// An item data object that represents a product or service.
    /// </summary>
    public class Item : IData, IKeyable<short>
    {
        public static readonly Item Empty = new Item();

        public const byte ID = 1, LATER = 2;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "下架"},
            {1, "上架"},
            {2, "推荐"},
            {3, "置顶"},
        };

        internal short id;
        internal string hubid;
        internal string name;
        internal string descr;
        internal string remark;
        internal string mov; // movie url
        internal string unit;
        internal decimal price;
        internal decimal fee; // delivery fee
        internal decimal shopp; // provision portion 
        internal decimal senderp; // shipping portion
        internal decimal teamp; // teaming portion
        internal short min;
        internal short step;
        internal bool refrig;
        internal short cap7; // total capacity in 7 days
        internal int shopid;
        internal short status;

        public short ongoing; // qty in progress

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(hubid), ref hubid);
            s.Get(nameof(name), ref name);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(remark), ref remark);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(refrig), ref refrig);
            s.Get(nameof(mov), ref mov);
            s.Get(nameof(price), ref price);
            s.Get(nameof(fee), ref fee);
            s.Get(nameof(shopp), ref shopp);
            s.Get(nameof(senderp), ref senderp);
            s.Get(nameof(teamp), ref teamp);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(shopid), ref shopid);
                s.Get(nameof(cap7), ref cap7);
                s.Get(nameof(status), ref status);
            }
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(hubid), hubid);
            s.Put(nameof(name), name);
            s.Put(nameof(descr), descr);
            s.Put(nameof(remark), remark);
            s.Put(nameof(unit), unit);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(refrig), refrig);
            s.Put(nameof(mov), mov);
            s.Put(nameof(price), price);
            s.Put(nameof(fee), fee);
            s.Put(nameof(shopp), shopp);
            s.Put(nameof(senderp), senderp);
            s.Put(nameof(teamp), teamp);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(shopid), shopid);
                s.Put(nameof(cap7), cap7);
                s.Put(nameof(status), status);
            }
        }

        public short Key => id;

        public short Avail => (short) (cap7 - ongoing);

        public override string ToString() => name;
    }
}