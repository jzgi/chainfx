using Greatbone;

namespace Samp
{
    /// <summary>
    /// An item data object that represents a product or service.
    /// </summary>
    public class Item : IData, IKeyable<string>
    {
        public static readonly Item Empty = new Item();

        public const byte PK = 1, LATER = 2;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "下架"},
            {1, "上架"},
            {2, "推荐"},
            {3, "置顶"},
        };

        internal string regid;
        internal string name;
        internal string descr;
        internal string remark;
        internal string mov; // movie url
        internal string unit;
        internal decimal price;
        internal decimal giverp; // giver price
        internal decimal dvrerp; // deliver cost
        internal decimal grperp; // grouper cost
        internal short min;
        internal short step;
        internal bool refrig;

        internal short demand;
        internal int giverid;
        internal short[] cap7;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) > 0)
            {
                s.Get(nameof(regid), ref regid);
                s.Get(nameof(name), ref name);
            }
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(remark), ref remark);
            s.Get(nameof(mov), ref mov);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(giverp), ref giverp);
            s.Get(nameof(dvrerp), ref dvrerp);
            s.Get(nameof(grperp), ref grperp);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(refrig), ref refrig);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(demand), ref demand);
                s.Get(nameof(giverid), ref giverid);
                s.Get(nameof(cap7), ref cap7);
                s.Get(nameof(status), ref status);
            }
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & PK) > 0)
            {
                s.Put(nameof(name), name);
            }
            s.Put(nameof(descr), descr);
            s.Put(nameof(remark), remark);
            s.Put(nameof(mov), mov);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(giverp), giverp);
            s.Put(nameof(dvrerp), dvrerp);
            s.Put(nameof(grperp), grperp);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(refrig), refrig);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(demand), demand);
                s.Put(nameof(giverid), giverid);
                s.Put(nameof(cap7), cap7);
                s.Put(nameof(status), status);
            }
        }

        public string Key => name;
    }
}