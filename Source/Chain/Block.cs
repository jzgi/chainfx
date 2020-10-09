using System;

namespace Skyiah.Chain
{
    public class Block : IData
    {
        public static readonly Block Empty = new Block();

        internal string peerid;
        internal int seq;

        internal string prevhash;
        internal string hash;
        internal DateTime stamp;
        internal string recs;
        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(peerid), ref peerid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(prevhash), ref prevhash);
            s.Get(nameof(hash), ref hash);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(recs), ref recs);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(peerid), peerid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(prevhash), prevhash);
            s.Put(nameof(hash), hash);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(recs), recs);
            s.Put(nameof(status), status);
        }
    }

    public class BlockRecord : IData
    {
        public static readonly BlockRecord Empty = new BlockRecord();

        internal string peerid;
        internal int seq;
        internal short idx;
        internal string hash;
        internal DateTime stamp;

        internal string txpeerid;
        internal int txno;

        internal short typ;
        internal string key;
        internal string descr;
        internal decimal amt;
        internal decimal balance;
        internal IData doc;
        internal string rtpeerid;

        internal short state;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(peerid), ref peerid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(idx), ref idx);
            s.Get(nameof(hash), ref hash);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(txpeerid), ref txpeerid);
            s.Get(nameof(txno), ref txno);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(key), ref key);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(balance), ref balance);
            s.Get(nameof(rtpeerid), ref rtpeerid);
            s.Get(nameof(state), ref state);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(peerid), peerid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(idx), idx);
            s.Put(nameof(hash), hash);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(txpeerid), txpeerid);
            s.Put(nameof(txno), txno);
            s.Put(nameof(typ), typ);
            s.Put(nameof(key), key);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(balance), balance);
            s.Put(nameof(rtpeerid), rtpeerid);
            s.Put(nameof(state), state);
        }
    }
}