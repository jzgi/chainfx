using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// An order data object.
    ///
    public class Order : IData
    {
        public static readonly Order Empty = new Order();

        internal int id;

        internal DateTime time;

        internal string shopid;

        internal string buyerid; // wechat id

        internal string buyer; // wechat name

        internal string tel;

        decimal total;

        Line[] lines;

        internal string payid; // payment id

        internal int status;

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(id), ref id);
            src.Get(nameof(shopid), ref shopid);
            src.Get(nameof(time), ref time);
            src.Get(nameof(buyerid), ref buyerid);
            src.Get(nameof(buyer), ref buyer);
            src.Get(nameof(tel), ref tel);
            src.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(id), id);
            snk.Put(nameof(shopid), shopid);
            snk.Put(nameof(time), time);
            snk.Put(nameof(buyerid), buyerid);
            snk.Put(nameof(tel), tel);
            snk.Put(nameof(status), status);
        }
    }

    public struct Line
    {
        string item;

        short qty;

        decimal price;

        decimal discount;

        public decimal Actual => price * qty - discount;

    }

}