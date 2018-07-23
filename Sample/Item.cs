using Greatbone;

namespace Samp
{
    /// <summary>
    /// An item data object that represents a product or service.
    /// </summary>
    public class Item : IData, IGroupKeyable<(string, string)>
    {
        public static readonly Item Empty = new Item();

        public const byte PK = 1, LATER = 2;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "下架"},
            {1, "上架"},
            {2, "推荐"},
        };

        internal string ctrid;
        internal string name;
        internal string descr;
        internal string unit;
        internal decimal price;
        internal short min;
        internal short step;
        internal bool refrig;
        internal string video; // video url
        internal string vdrid; // vendor that provides the item
        internal short demand;
        internal short[] cap7; // capacity of week
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Get(nameof(ctrid), ref ctrid);
                s.Get(nameof(name), ref name);
            }
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(refrig), ref refrig);
            s.Get(nameof(video), ref video);
            s.Get(nameof(vdrid), ref vdrid);
            s.Get(nameof(demand), ref demand);
            s.Get(nameof(cap7), ref cap7);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Put(nameof(ctrid), ctrid);
                s.Put(nameof(name), name);
            }
            s.Put(nameof(descr), descr);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(refrig), refrig);
            s.Put(nameof(video), video);
            s.Put(nameof(vdrid), vdrid);
            s.Put(nameof(demand), demand);
            s.Put(nameof(cap7), cap7);
            s.Put(nameof(status), status);
        }

        public (string, string) Key => (ctrid, name);

        public bool GroupAs((string, string) akey)
        {
            return ctrid == akey.Item1;
        }
    }
}