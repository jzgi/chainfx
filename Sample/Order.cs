using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// An order data object.
    ///
    public class Order : IDat
    {
        public static readonly Order Empty = new Order();

        internal int id;
        internal string shopid;
        internal DateTime time;
        internal string buyerid; // wechat id
        internal string buyer; // wechat name
        internal string tel;
        decimal total;

        internal string payid; // payment id
        internal int status;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(shopid), ref shopid);
            s.Get(nameof(time), ref time);
            s.Get(nameof(buyerid), ref buyerid);
            s.Get(nameof(buyer), ref buyer);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(shopid), shopid);
            s.Put(nameof(time), time);
            s.Put(nameof(buyerid), buyerid);
            s.Put(nameof(tel), tel);
            s.Put(nameof(status), status);
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