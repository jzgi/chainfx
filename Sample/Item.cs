using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A product or service being sold.
    ///
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        internal string shopid;
        internal string name;
        internal string unit;
        internal string descr;
        internal ArraySegment<byte> icon;
        internal decimal oprice; // original price
        internal decimal price; // current price
        internal short capacity;
        internal int min; // minimal ordered
        internal int step;
        internal int sold; // total sold 
        internal bool enabled;

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Prime())
            {
                i.Get(nameof(shopid), ref shopid);
            }
            i.Get(nameof(name), ref name);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(descr), ref descr);
            if (proj.Bin())
            {
                i.Get(nameof(icon), ref icon);
            }
            i.Get(nameof(oprice), ref oprice);
            i.Get(nameof(price), ref price);
            i.Get(nameof(min), ref min);
            i.Get(nameof(step), ref step);
            if (proj.Froz())
            {
                i.Get(nameof(sold), ref sold);
            }
            i.Get(nameof(enabled), ref enabled);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Power())
            {
                o.Put(nameof(shopid), shopid);
            }
            o.Put(nameof(name), name, Label: "品名", Max: 10, Required: true);
            o.Put(nameof(unit), unit);
            o.Put(nameof(descr), descr);
            if (proj.Bin())
            {
                o.Put(nameof(icon), icon, Label: "图片", Size: "240,240", Ratio: "1:1", Required: true);
            }
            o.Put(nameof(oprice), oprice);
            o.Put(nameof(price), price, Required: true);
            o.Put(nameof(min), min);
            o.Put(nameof(step), step);
            if (proj.Froz())
            {
                o.Put(nameof(sold), sold);
            }
            o.Put(nameof(enabled), enabled, Opt: b => b ? "在售" : "下架");
        }
    }
}