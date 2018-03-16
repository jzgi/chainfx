using System;
using Greatbone;

namespace Core
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
        internal string orgid;
        internal DateTime fro;
        internal DateTime till;
        internal int orders;
        internal decimal total;
        internal decimal cash;
        internal string payer;
        internal string err;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(orgid), ref orgid);
            s.Get(nameof(fro), ref fro);
            s.Get(nameof(till), ref till);
            s.Get(nameof(orders), ref orders);
            s.Get(nameof(total), ref total);
            s.Get(nameof(cash), ref cash);
            s.Get(nameof(payer), ref payer);
            s.Get(nameof(err), ref err);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(orgid), orgid);
            s.Put(nameof(fro), fro);
            s.Put(nameof(till), till);
            s.Put(nameof(orders), orders);
            s.Put(nameof(total), total);
            s.Put(nameof(cash), cash);
            s.Put(nameof(payer), payer);
            s.Put(nameof(err), err);
            s.Put(nameof(status), status);
        }
    }
}