using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// A repay data object.
    ///
    public class Repay : IData
    {
        public const ushort PAYING = 1;

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

        internal int id;
        internal string shopid;
        internal string shop;
        internal DateTime till;
        internal int orders;
        internal decimal total;
        internal decimal cash;
        internal DateTime paid;
        internal string payer;
        internal short status;
        internal string err;

        public void Read(IDataInput i, ushort proj = 0x00ff)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shop), ref shop);
            i.Get(nameof(orders), ref orders);
            i.Get(nameof(total), ref total);
            i.Get(nameof(cash), ref cash);
            i.Get(nameof(till), ref till);
            if ((proj & PAYING) == PAYING)
            {
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(payer), ref payer);
            }
            i.Get(nameof(status), ref status);
            i.Get(nameof(err), ref err);
        }

        public void Write<R>(IDataOutput<R> o, ushort proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Group("商家");
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shop), shop);
            o.UnGroup();
            o.Put(nameof(till), till, "截至日期");
            o.Put(nameof(orders), orders, "订单数");
            o.Put(nameof(total), total, "订单总额");
            o.Put(nameof(cash), cash, "转款金额");
            if ((proj & PAYING) == PAYING)
            {
                o.Group("转款操作");
                o.Put(nameof(paid), paid);
                o.Put(nameof(payer), payer);
                o.UnGroup();
                o.Put(nameof(err), err, "出错提示");
            }
            o.Put(nameof(status), status, "状态", STATUS);
        }
    }
}