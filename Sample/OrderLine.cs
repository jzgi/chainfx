using Greatbone.Core;
using static Greatbone.Core.FlagsUtility;

namespace Greatbone.Sample
{
    public struct OrderLine : IData
    {
        string shopid;

        string item;

        short qty;

        string unit;

        decimal oprice;

        decimal price;

        string note;

        public decimal Subtotal => price * qty;

        public void ReadData(IDataInput i, byte flags = 0)
        {
            if (flags.Has(KEPT))
            {
                i.Get(nameof(shopid), ref shopid);
            }
            i.Get(nameof(item), ref item);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(oprice), ref oprice);
            i.Get(nameof(price), ref price);
            i.Get(nameof(note), ref note);
        }

        public void WriteData<R>(IDataOutput<R> o, byte flags = 0) where R : IDataOutput<R>
        {
            if (flags.Has(KEPT))
            {
                o.Put(nameof(shopid), shopid);
            }
            o.Put(nameof(item), item);
            o.Put(nameof(qty), qty);
            o.Put(nameof(unit), unit);
            o.Put(nameof(oprice), oprice);
            o.Put(nameof(price), price);
            o.Put(nameof(note), note);
        }
    }
}