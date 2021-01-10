using System;

namespace SkyChain.Chain
{
    /// <summary>
    /// An archival record.
    /// </summary>
    public class Arch : IData, IKeyable<long>
    {
        public static readonly Arch Empty = new Arch();

        public const byte INSTALL = 0x10, INTEGRITY = 0x20;

        internal short peerid;
        internal long seq;
        internal long chk;
        internal long blockchk;

        internal long job; // job + step is globally-unique op number
        internal short step;
        internal string acct;
        internal string name;
        internal string ldgr;
        internal string descr;
        internal decimal amt;
        internal decimal bal;
        internal JObj doc;
        internal DateTime stated;
        internal DateTime stamp;

        public void Read(ISource s, byte proj = 15)
        {
            if ((proj & INSTALL) == INSTALL)
            {
                s.Get(nameof(peerid), ref peerid);
                s.Get(nameof(seq), ref seq);
            }
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
            s.Get(nameof(stamp), ref stamp);
            if ((proj & INTEGRITY) == INTEGRITY)
            {
                s.Get(nameof(chk), ref chk);
                s.Get(nameof(blockchk), ref blockchk);
            }
        }

        public void Write(ISink s, byte proj = 15)
        {
            if ((proj & INSTALL) == INSTALL)
            {
                s.Put(nameof(peerid), peerid);
                s.Put(nameof(seq), seq);
            }
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
            s.Put(nameof(stamp), stamp);
            if ((proj & INTEGRITY) == INTEGRITY)
            {
                s.Put(nameof(chk), chk);
                s.Put(nameof(blockchk), blockchk);
            }
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