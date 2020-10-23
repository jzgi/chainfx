using System;

namespace SkyChain.Chain
{
    public class BlockRecord : IData
    {
        public static readonly BlockRecord Empty = new BlockRecord();

        internal string peerid;
        internal int seq;
        internal string acct;
        internal short typ;
        internal int caseid;
        internal DateTime time;

        internal string descr;
        internal decimal amt;
        internal decimal bal;
        internal JObj doc;
        internal short digest;
        internal string rpeerid;
        internal string racct;


        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(peerid), ref peerid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(caseid), ref caseid);
            s.Get(nameof(time), ref time);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(bal), ref bal);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(digest), ref digest);
            s.Get(nameof(rpeerid), ref rpeerid);
            s.Get(nameof(racct), ref racct);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(peerid), peerid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(acct), acct);
            s.Put(nameof(typ), typ);
            s.Put(nameof(caseid), caseid);
            s.Put(nameof(time), time);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(bal), bal);
            s.Put(nameof(doc), doc);
            s.Put(nameof(digest), digest);
            s.Put(nameof(rpeerid), rpeerid);
            s.Put(nameof(racct), racct);
        }
    }
}