using System;

namespace CloudUn.Net
{
    public class Peer : IData
    {
        internal string id;

        internal string name;

        internal string raddr;

        internal DateTime stamp;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(raddr), ref raddr);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(raddr), raddr);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(status), status);
        }
    }
}