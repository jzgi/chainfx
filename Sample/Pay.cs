using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Pay : IData
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

        public void ReadData(IDataInput i, ushort flags = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(time), ref time);
            i.Get(nameof(custid), ref custid);
            i.Get(nameof(cust), ref cust);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort flags = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(time), time);
            o.Put(nameof(custid), custid);
            o.Put(nameof(tel), tel);
            o.Put(nameof(status), status);
        }

    }

}