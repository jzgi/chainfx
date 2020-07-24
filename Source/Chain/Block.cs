using System;

namespace SkyChain.Chain
{
    public class Block : IData
    {
        public static readonly Block Empty = new Block();

        internal string nodeid;
        internal int seq;

        internal string hash;
        internal DateTime stamp;
        internal string creator;
        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(nodeid), ref nodeid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(hash), ref hash);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(creator), ref creator);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(nodeid), nodeid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(hash), hash);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(creator), creator);
            s.Put(nameof(status), status);
        }
    }

    public class BlockLine : IData
    {
        public static readonly BlockLine Empty = new BlockLine();

        internal string nodeid;
        internal int seq;
        internal short idx;

        internal short typ;
        internal string key;
        internal DateTime stamp;

        internal string[] tags;
        internal JObj content;

        internal string hash;
        internal short state;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(nodeid), ref nodeid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(idx), ref idx);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(key), ref key);
            s.Get(nameof(tags), ref tags);
            s.Get(nameof(content), ref content);
            s.Get(nameof(hash), ref hash);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(state), ref state);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(nodeid), nodeid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(idx), idx);
            s.Put(nameof(typ), typ);
            s.Put(nameof(key), key);
            s.Put(nameof(tags), tags);
            s.Put(nameof(content), content);
            s.Put(nameof(hash), hash);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(state), state);
        }
    }
}