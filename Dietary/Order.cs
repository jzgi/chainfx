using System;
using Greatbone.Core;

namespace Ministry.Dietary
{

    /// <summary>
    /// </summary>
    public class Order : IBean
    {
        internal int id;
        internal string shopid;
        internal DateTime time;
        internal string custid; // wechat id
        internal string cust; // wechat name
        internal string tel;
        decimal total;

        internal string payid; // payment id
        internal int status;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(shopid), ref shopid);
            s.Get(nameof(time), ref time);
            s.Get(nameof(custid), ref custid);
            s.Get(nameof(cust), ref cust);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(shopid), shopid);
            s.Put(nameof(time), time);
            s.Put(nameof(custid), custid);
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