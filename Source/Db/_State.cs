using System;

namespace SkyChain.Db
{
    /// <summary>
    /// An abstract record state, can serve as a model for an OP VIEW.
    /// </summary>
    public class _State : IData
    {
        public static readonly _State Empty = new _State();

        internal string acct;

        internal string name;

        internal string tip;

        internal decimal amt;

        internal DateTime stamp;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tip), ref tip);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(stamp), ref stamp);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(acct), acct);
            s.Put(nameof(name), name);
            s.Put(nameof(tip), tip);
            s.Put(nameof(amt), amt);
            s.Put(nameof(stamp), stamp);
        }

        public string Acct => acct;

        public string Name => name;

        public string Tip => tip;

        public decimal Amt => amt;

        public DateTime Stamp => stamp;
    }
}