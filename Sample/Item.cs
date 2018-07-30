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

        // sorts
        public static readonly Map<short, string> Sorts = new Map<short, string>
        {
            {1, "粉面类"},
            {11, "谷物类"},
            {21, "其他类"},
        };

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "下架"},
            {1, "上架"},
            {2, "推荐"},
        };

        internal string name;
        internal string descr;
        internal string remark;
        internal string mov; // movie url
        internal short sort;
        internal string unit;
        internal decimal price;
        internal decimal _sup; // supply price
        internal decimal _grp; // group cost
        internal decimal _dvr; // deliver cost
        internal short min;
        internal short step;
        internal bool refrig;
        internal string supid; // supply of the item
        internal short demand;
        internal short[] cap7; // capacity of week
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Get(nameof(name), ref name);
            }
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(remark), ref remark);
            s.Get(nameof(mov), ref mov);
            s.Get(nameof(sort), ref sort);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(_sup), ref _sup);
            s.Get(nameof(_grp), ref _grp);
            s.Get(nameof(_dvr), ref _dvr);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(refrig), ref refrig);
            s.Get(nameof(supid), ref supid);
            s.Get(nameof(demand), ref demand);
            s.Get(nameof(cap7), ref cap7);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Put(nameof(name), name);
            }
            s.Put(nameof(descr), descr);
            s.Put(nameof(remark), remark);
            s.Put(nameof(mov), mov);
            s.Put(nameof(sort), sort);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(_sup), _sup);
            s.Put(nameof(_grp), _grp);
            s.Put(nameof(_dvr), _dvr);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(refrig), refrig);
            s.Put(nameof(supid), supid);
            s.Put(nameof(demand), demand);
            s.Put(nameof(cap7), cap7);
            s.Put(nameof(status), status);
        }

        public string Key => name;
    }
}