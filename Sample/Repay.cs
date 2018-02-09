using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// A repay data object.
    ///
    public class Repay : IData
    {
        public static readonly Repay Empty = new Repay();

        // status
        public const short CREATED = 0, PAID = 1;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {CREATED, "新结算"},
            {PAID, "已转款"}
        };

        internal int id;
        internal string shopid;
        internal DateTime fro;
        internal DateTime till;
        internal int orders;
        internal decimal total;
        internal decimal cash;
        internal string payer;
        internal short status;
        internal string err;

        public void Read(IDataInput i, byte proj = 0x0f)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(fro), ref fro);
            i.Get(nameof(till), ref till);
            i.Get(nameof(orders), ref orders);
            i.Get(nameof(total), ref total);
            i.Get(nameof(cash), ref cash);
            i.Get(nameof(payer), ref payer);
            i.Get(nameof(status), ref status);
            i.Get(nameof(err), ref err);
        }

        public void Write<R>(IDataOutput<R> o, byte proj = 0x0f) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(fro), fro);
            o.Put(nameof(till), till);
            o.Put(nameof(orders), orders);
            o.Put(nameof(total), total);
            o.Put(nameof(cash), cash);
            o.Put(nameof(payer), payer);
            o.Put(nameof(status), status);
        }
    }
}