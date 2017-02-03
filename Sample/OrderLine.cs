using Greatbone.Core;

namespace Greatbone.Sample
{
    public struct OrderLine : IData
    {
        internal string shopid;

        internal string item;

        internal short qty;

        internal decimal price;

        string note;

        public decimal Subtotal => price * qty;

        public void ReadData(IDataInput i, ushort proj = 0)
        {
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(item), ref item);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(price), ref price);
            i.Get(nameof(note), ref note);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(item), item);
            o.Put(nameof(qty), qty);
            o.Put(nameof(price), price);
            o.Put(nameof(note), note);
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}