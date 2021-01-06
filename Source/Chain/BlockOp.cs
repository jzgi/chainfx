using System;

namespace SkyChain.Chain
{
    public class BlockOp : IData, IKeyable<long>
    {
        public static readonly BlockOp Empty = new BlockOp();

        // globally-unique op number
        internal long job;
        internal short step;

        internal string acct;
        internal string name;
        internal string ldgr;
        internal string descr;
        internal decimal amt;
        internal decimal bal;
        internal JObj doc;
        internal DateTime stated;

        internal long dgst;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(job), ref job);
            s.Get(nameof(step), ref step);
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(name), ref name);
            s.Get(nameof(ldgr), ref ldgr);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(bal), ref bal);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(stated), ref stated);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(job), job);
            s.Put(nameof(step), step);
            s.Put(nameof(acct), acct);
            s.Put(nameof(name), name);
            s.Put(nameof(ldgr), ldgr);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(bal), bal);
            s.Put(nameof(doc), doc);
            s.Put(nameof(stated), stated);
        }

        public long Job => job;

        public short Step => step;

        public string Acct => acct;

        public string Name => name;

        public string Ldgr => ldgr;

        public string Descr => descr;

        public decimal Amt => amt;

        public decimal Bal => bal;

        public JObj Doc => doc;

        public DateTime Stated => stated;

        public long Key => job;
    }
}