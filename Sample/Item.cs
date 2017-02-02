using Greatbone.Core;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    /// 
    /// An item that is sold. 
    ///
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        internal string shopid;

        internal string name;

        internal string unit;

        internal string descript;

        internal byte[] icon;

        internal decimal oprice; // original price

        internal string price; // actual price, may be discounted

        internal int min; // minimal quantity ordered

        internal int step;

        internal int sold; // total sold 

        public void ReadData(IDataInput i, ushort proj = 0)
        {
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(name), ref name);
            i.Get(nameof(unit), ref unit);
            if (proj.Bin())
            {
                i.Get(nameof(icon), ref icon);
            }
            i.Get(nameof(oprice), ref oprice);
            i.Get(nameof(price), ref price);
            i.Get(nameof(min), ref min);
            i.Get(nameof(step), ref step);
            i.Get(nameof(sold), ref sold);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(name), name);
            o.Put(nameof(unit), unit);
            if (proj.Bin())
            {
                o.Put(nameof(icon), icon);
            }
            o.Put(nameof(oprice), oprice);
            o.Put(nameof(price), price);
            o.Put(nameof(min), min);
            o.Put(nameof(step), step);
            o.Put(nameof(sold), sold);
        }
    }
}