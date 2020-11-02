using System;

namespace SkyChain.Chain
{
    public class Record : IData
    {
        public static readonly Record Empty = new Record();

        internal string tn; // transaction number
        internal short step;
        internal string an; // account number
        internal short typ;
        internal string @case;
        internal string descr;
        internal decimal amt;
        internal decimal bal;
        internal JObj doc;
        internal DateTime stamp;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(tn), ref tn);
            s.Get(nameof(step), ref step);
            s.Get(nameof(an), ref an);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(@case), ref @case);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(bal), ref bal);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(stamp), ref stamp);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(tn), tn);
            s.Put(nameof(step), step);
            s.Put(nameof(an), an);
            s.Put(nameof(typ), typ);
            s.Put(nameof(@case), @case);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(bal), bal);
            s.Put(nameof(doc), doc);
            s.Put(nameof(stamp), stamp);
        }

        public string Case => @case;

        public string Description => descr;

        public decimal Ammount => amt;

        public decimal Balance => bal;

        public DateTime Stamp => stamp;
    }
}