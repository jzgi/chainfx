using System;
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
        public const short CREATED = 0, PAID = 1;

        // status
        static readonly Map<short, string> STATUS = new Map<short, string>
        {
            {0, "新创建/未转款"},
            {1, "已转款"}
        };


        public static readonly Repay Empty = new Repay();

        internal int id;
        internal short shopid;
        internal string shopname;
        internal DateTime till;
        internal int orders;
        internal decimal total;
        internal decimal cash;
        internal DateTime paid;
        internal string payer;
        internal short status;
        internal string err;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(orders), ref orders);
            i.Get(nameof(total), ref total);
            i.Get(nameof(cash), ref cash);
            i.Get(nameof(till), ref till);
            if ((proj & PAY) == PAY)
            {
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(payer), ref payer);
            }
            i.Get(nameof(status), ref status);
            i.Get(nameof(err), ref err);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.Put(nameof(till), till);
            o.Put(nameof(orders), orders);
            o.Put(nameof(total), total);
            o.Put(nameof(cash), cash);
            if ((proj & PAY) == PAY)
            {
                o.Put(nameof(paid), paid);
                o.Put(nameof(payer), payer);
                o.Put(nameof(err), err);
            }
            o.Put(nameof(status), status);
        }
    }
}