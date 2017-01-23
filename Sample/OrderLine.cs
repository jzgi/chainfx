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

        public void Load(ISource src, byte flags = 0)
        {
            if (flags.Has(KEPT))
            {
                src.Get(nameof(shopid), ref shopid);
            }
            src.Get(nameof(item), ref item);
            src.Get(nameof(qty), ref qty);
            src.Get(nameof(unit), ref unit);
            src.Get(nameof(oprice), ref oprice);
            src.Get(nameof(price), ref price);
            src.Get(nameof(note), ref note);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            if (flags.Has(KEPT))
            {
                snk.Put(nameof(shopid), shopid);
            }
            snk.Put(nameof(item), item);
            snk.Put(nameof(qty), qty);
            snk.Put(nameof(unit), unit);
            snk.Put(nameof(oprice), oprice);
            snk.Put(nameof(price), price);
            snk.Put(nameof(note), note);
        }
    }
}