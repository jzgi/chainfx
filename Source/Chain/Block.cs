using System;

namespace SkyChain.Chain
{
    public class Block : IData
    {
        public static readonly Block Empty = new Block();

        internal string peerid;
        internal int seq;
        internal string prevtag;
        internal string tag;
        internal DateTime stamp;
        internal string recs;
        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(peerid), ref peerid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(prevtag), ref prevtag);
            s.Get(nameof(tag), ref tag);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(recs), ref recs);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(peerid), peerid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(prevtag), prevtag);
            s.Put(nameof(tag), tag);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(recs), recs);
            s.Put(nameof(status), status);
        }
    }
}