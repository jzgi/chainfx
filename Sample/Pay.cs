using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Pay : IDat
    {
        internal string id;
        internal string shopid;
        internal DateTime time;
        internal string custid; // wechat id
        internal string cust; // wechat name
        internal string tel;
        decimal total;

        internal string payid; // payment id
        internal int status;

        public void Load(ISource src, byte bits = 0)
        {
            src.Get(nameof(id), ref id);
            src.Get(nameof(shopid), ref shopid);
            src.Get(nameof(time), ref time);
            src.Get(nameof(custid), ref custid);
            src.Get(nameof(cust), ref cust);
            src.Get(nameof(tel), ref tel);
            src.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>
        {
            snk.Put(nameof(id), id);
            snk.Put(nameof(shopid), shopid);
            snk.Put(nameof(time), time);
            snk.Put(nameof(custid), custid);
            snk.Put(nameof(tel), tel);
            snk.Put(nameof(status), status);
        }

    }

}