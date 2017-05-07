using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// A repay data object.
    ///
    public class Repay : IData
    {
        public const short PAY = 1;

        // status
        public const short
            CREATED = 0,
            PAID = 1;

        // status
        static readonly Dictionary<short, string> STATUS = new Dictionary<short, string>
        {
            [0] = "新创建",
            [1] = "已付款"
        };


        public static readonly Repay Empty = new Repay();

        internal int term; // platform shop id
        internal string shopid;
        internal string city;
        internal string mgrwx;
        internal string shopname;

        internal int orders;
        internal decimal total;
        internal decimal amount;
        internal DateTime till;
        internal DateTime created;
        internal string creator;

        internal DateTime paid;
        internal string payer;
        internal short status;

        public void ReadData(IDataInput i, short proj = 0)
        {
            i.Get(nameof(term), ref term);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(city), ref city);
            i.Get(nameof(mgrwx), ref mgrwx);

            i.Get(nameof(orders), ref orders);
            i.Get(nameof(total), ref total);
            i.Get(nameof(amount), ref amount);
            i.Get(nameof(till), ref till);
            i.Get(nameof(created), ref created);
            i.Get(nameof(creator), ref creator);

            if ((proj & PAY) == PAY)
            {
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(payer), ref payer);
            }

            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(term), term);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.Put(nameof(city), city);
            o.Put(nameof(mgrwx), mgrwx);

            o.Put(nameof(orders), orders);
            o.Put(nameof(total), total);
            o.Put(nameof(amount), amount);
            o.Put(nameof(till), till);
            o.Put(nameof(created), created);
            o.Put(nameof(creator), creator);
            if ((proj & PAY) == PAY)
            {
                o.Put(nameof(paid), paid);
                o.Put(nameof(payer), payer);
            }
            o.Put(nameof(status), status);
        }
    }
}