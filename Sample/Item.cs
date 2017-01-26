using Greatbone.Core;
using static Greatbone.Core.FlagsUtility;

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

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(shopid), ref shopid);
            src.Get(nameof(name), ref name);
            src.Get(nameof(unit), ref unit);
            if (flags.Has(BINARY))
            {
                src.Get(nameof(icon), ref icon);
            }
            src.Get(nameof(oprice), ref oprice);
            src.Get(nameof(price), ref price);
            src.Get(nameof(min), ref min);
            src.Get(nameof(step), ref step);
            src.Get(nameof(sold), ref sold);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(shopid), shopid);
            snk.Put(nameof(name), name);
            snk.Put(nameof(unit), unit);
            if (flags.Has(BINARY))
            {
                snk.Put(nameof(icon), icon);
            }
            snk.Put(nameof(oprice), oprice);
            snk.Put(nameof(price), price);
            snk.Put(nameof(min), min);
            snk.Put(nameof(step), step);
            snk.Put(nameof(sold), sold);
        }
    }
}