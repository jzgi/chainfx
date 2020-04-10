using System;

namespace SkyCloud.Chain
{
    public class Block : IData
    {
        public static readonly Block Empty = new Block();

        internal string aid;

        internal int seq;

        internal string bid;

        internal short typid;

        internal string key;

        internal string[] tags;

        internal byte[] body;

        internal string hash;

        internal DateTime stamp;

        internal string loginid;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(aid), ref aid);
            s.Get(nameof(seq), ref seq);
            s.Get(nameof(bid), ref bid);
            s.Get(nameof(typid), ref typid);
            s.Get(nameof(key), ref key);
            s.Get(nameof(tags), ref tags);
            s.Get(nameof(body), ref body);
            s.Get(nameof(hash), ref hash);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(loginid), ref loginid);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(aid), aid);
            s.Put(nameof(seq), seq);
            s.Put(nameof(bid), bid);
            s.Put(nameof(typid), typid);
            s.Put(nameof(key), key);
            s.Put(nameof(tags), tags);
            s.Put(nameof(body), body);
            s.Put(nameof(hash), hash);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(loginid), loginid);
            s.Put(nameof(status), status);
        }
    }
}