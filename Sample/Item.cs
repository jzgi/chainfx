using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    /// 
    /// An item that is sold. 
    ///
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        internal string shopid;
        internal string item;
        internal byte[] icon;
        internal decimal price;
        internal string remark;
        internal string tel;
        internal int sold;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(item), ref item);
            s.Get(nameof(shopid), ref shopid);
            if (z.Ya(BIN)) s.Get(nameof(icon), ref icon);
            s.Get(nameof(price), ref price);
            s.Get(nameof(remark), ref remark);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(sold), ref sold);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(item), item);
            s.Put(nameof(shopid), shopid);
            if (z.Ya(BIN)) s.Put(nameof(icon), icon);
            s.Put(nameof(price), price);
            s.Put(nameof(tel), tel);
            s.Put(nameof(sold), sold);
        }
    }
}