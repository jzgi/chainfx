using System;

namespace CloudUn.Net
{
    public class Block : IData
    {
        internal string entid;

        internal int seq;

        internal short typid;

        internal string keyno;

        internal string[] tags;

        internal JObj content;

        internal string hash;

        internal DateTime stamp;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
        }
    }
}