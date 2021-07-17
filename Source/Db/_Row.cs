using System;

namespace SkyChain.Db
{
    /// <summary>
    /// An abstract account entry or record.
    /// </summary>
    public class _Row : IData
    {
        public static readonly _Row Empty = new _Row();

        public const byte ID = 1, LATER = 2;

        // globally unique transaction number that can identify the source peer
        internal long txn;

        // typ pf ledger
        internal short typid;

        internal string acct;

        internal string name;

        internal string remark;

        internal decimal amt;

        // app-given time
        internal DateTime stamp;

        // remote peer's id
        internal short rpeerid;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(txn), ref txn);
            s.Get(nameof(typid), ref typid);
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(name), ref name);
            s.Get(nameof(remark), ref remark);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(rpeerid), ref rpeerid);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(txn), txn);
            s.Put(nameof(typid), typid);
            s.Put(nameof(acct), acct);
            s.Put(nameof(name), name);
            s.Put(nameof(remark), remark);
            s.Put(nameof(amt), amt);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(rpeerid), rpeerid);
        }

        public long Txn => txn;

        public string Acct => acct;

        public string Name => name;

        public string Remark => remark;

        public decimal Amt => amt;

        public DateTime Stamp => stamp;
    }
}