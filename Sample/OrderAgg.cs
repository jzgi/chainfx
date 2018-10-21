using Greatbone;

namespace Samp
{
    public class OrderAgg : IData
    {
        // the meaning of no varies according to situations
        internal short no;

        internal short itemid;
        internal string itemname;
        internal short qty;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(no), ref no);
            s.Get(nameof(itemid), ref itemid);
            s.Get(nameof(itemname), ref itemname);
            s.Get(nameof(qty), ref qty);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(no), no);
            s.Put(nameof(itemid), itemid);
            s.Put(nameof(itemname), itemname);
            s.Put(nameof(qty), qty);
        }
    }
}