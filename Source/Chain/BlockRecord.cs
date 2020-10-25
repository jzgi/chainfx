using System;

namespace SkyChain.Chain
{
    public class BlockRecord : IData
    {
        public static readonly BlockRecord Empty = new BlockRecord();

        internal string peerid;
        internal int seq;

        internal string tn; // transaction number
        internal short step;

        internal string an; // account number
        internal short typ;
        internal int inst;

        internal string descr;
        internal decimal amt;
        internal decimal bal;
        internal JObj doc;
        internal DateTime stamp;
        internal short digest;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(peerid), ref peerid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(tn), ref tn);
            s.Get(nameof(step), ref step);
            s.Get(nameof(an), ref an);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(inst), ref inst);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(bal), ref bal);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(digest), ref digest);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(peerid), peerid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(tn), tn);
            s.Put(nameof(step), step);
            s.Put(nameof(an), an);
            s.Put(nameof(typ), typ);
            s.Put(nameof(inst), inst);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(bal), bal);
            s.Put(nameof(doc), doc);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(digest), digest);
        }
    }
}