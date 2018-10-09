using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A repay data object.
    ///  </summary>
    public class Repay : IData, IKeyable<int>
    {
        public static readonly Repay Empty = new Repay();

        public static readonly Map<short, string> Jobs = new Map<short, string>
        {
            {1, "供货"},
            {2, "派送"},
            {3, "团组"}
        };

        // status
        public const short CREATED = 0, FAILED = 1, PAID = 2;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {CREATED, "新创建"},
            {FAILED, "转款失败"},
            {PAID, "已转款"}
        };

        internal int id;
        internal string hubid;
        internal short job;
        internal int uid;
        internal string uname;
        internal string uwx;
        internal DateTime fro;
        internal DateTime till;
        internal int orders;
        internal decimal cash;
        internal DateTime paid;
        internal string payer;
        internal string err;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(hubid), ref hubid);
            s.Get(nameof(job), ref job);
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(uwx), ref uwx);
            s.Get(nameof(fro), ref fro);
            s.Get(nameof(till), ref till);
            s.Get(nameof(orders), ref orders);
            s.Get(nameof(cash), ref cash);
            s.Get(nameof(paid), ref paid);
            s.Get(nameof(payer), ref payer);
            s.Get(nameof(err), ref err);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(hubid), hubid);
            s.Put(nameof(job), job);
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(uwx), uwx);
            s.Put(nameof(fro), fro);
            s.Put(nameof(till), till);
            s.Put(nameof(orders), orders);
            s.Put(nameof(cash), cash);
            s.Put(nameof(paid), paid);
            s.Put(nameof(payer), payer);
            s.Put(nameof(err), err);
            s.Put(nameof(status), status);
        }

        public int Key => id;
    }
}