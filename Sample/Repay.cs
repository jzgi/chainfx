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

        // status
        public const short CREATED = 0, FAILED = 1, PAID = 2;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {CREATED, null},
            {FAILED, "失败"},
            {PAID, "已转"}
        };

        internal int id;
        internal string hubid;
        internal short shopid;
        internal short teamid;
        internal int userid;
        internal string user;
        internal string userwx; // openid for money transfer
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
            s.Get(nameof(shopid), ref shopid);
            s.Get(nameof(teamid), ref teamid);
            s.Get(nameof(userid), ref userid);
            s.Get(nameof(user), ref user);
            s.Get(nameof(userwx), ref userwx);
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
            s.Put(nameof(shopid), shopid);
            s.Put(nameof(teamid), teamid);
            s.Put(nameof(userid), userid);
            s.Put(nameof(user), user);
            s.Put(nameof(userwx), userwx);
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