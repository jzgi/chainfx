using System;

namespace SkyChain.Chain
{
    public class Block : IData
    {
        public static readonly Block Empty = new Block();

        public const byte ID = 1, EXTRA = 0x10;

        internal string peerid;
        internal int seq;
        internal short status;
        internal DateTime stamp;
        internal short sts;
        internal int pdgst;
        internal int dgst;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(peerid), ref peerid);
            if ((proj & ID) == ID)
            {
                s.Get(nameof(seq), ref seq);
            }
            s.Get(nameof(status), ref status);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(sts), ref sts);
            s.Get(nameof(pdgst), ref pdgst);
            s.Get(nameof(dgst), ref dgst);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(peerid), peerid);
            if ((proj & ID) == ID)
            {
                s.Put(nameof(seq), seq);
            }
            s.Put(nameof(status), status);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(sts), sts);
            s.Put(nameof(pdgst), pdgst);
            s.Put(nameof(dgst), dgst);
        }
    }
}