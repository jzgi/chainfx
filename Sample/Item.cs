using Greatbone;

namespace Core
{
    /// <summary>
    /// An item data object, that represents a product or service.
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

        internal string orgid;
        internal string name;
        internal string descr;
        internal string unit;
        internal decimal price;
        internal decimal comp; // compensation
        internal short min;
        internal short step;
        internal short stock;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Get(nameof(orgid), ref orgid);
                s.Get(nameof(name), ref name);
            }
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(comp), ref comp);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(stock), ref stock);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Put(nameof(orgid), orgid);
                s.Put(nameof(name), name);
            }
            s.Put(nameof(descr), descr);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(comp), comp);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(stock), stock);
            s.Put(nameof(status), status);
        }

        public (string, string) Key => (orgid, name);

        public bool GroupWith((string, string) akey)
        {
            return orgid == akey.Item1;
        }
    }
}