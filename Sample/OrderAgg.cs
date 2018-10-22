using Greatbone;

namespace Samp
{
    public class OrderAgg : IData
    {
        // the meaning of no varies according to situations
        internal short no;

        internal short itemid;
        internal string item;
        internal short qty;
        internal decimal cash;
        internal string[] oprs; // operators

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(no), ref no);
            s.Get(nameof(itemid), ref itemid);
            s.Get(nameof(item), ref item);
            s.Get(nameof(qty), ref qty);
            s.Get(nameof(cash), ref cash);
            s.Get(nameof(oprs), ref oprs);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(no), no);
            s.Put(nameof(itemid), itemid);
            s.Put(nameof(item), item);
            s.Put(nameof(qty), qty);
            s.Put(nameof(cash), cash);
            s.Put(nameof(oprs), oprs);
        }
    }
}