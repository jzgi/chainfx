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
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [0] = "新创建/未转款",
            [1] = "已转款"
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
        internal DateTime thru;
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
            i.Get(nameof(thru), ref thru);
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
            o.Group("商家");
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.UnGroup();
            o.Put(nameof(city), city);
            o.Put(nameof(mgrwx), mgrwx);

            o.Put(nameof(thru), thru, "截至日期");
            o.Put(nameof(orders), orders, "订单数");
            o.Put(nameof(total), total);
            o.Put(nameof(amount), amount, "合计金额");
            o.Group("转款操作");
            o.Put(nameof(created), created);
            o.Put(nameof(creator), creator);
            o.UnGroup();
            if ((proj & PAY) == PAY)
            {
                o.Put(nameof(paid), paid);
                o.Put(nameof(payer), payer);
            }
            o.Put(nameof(status), status, "状态", STATUS);
        }
    }
}