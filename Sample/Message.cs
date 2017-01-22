using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public struct Message : IData
    {
        internal string fromid;

        internal string from;

        internal short type;

        internal string text;

        internal DateTime time;

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(fromid), ref fromid);
            src.Get(nameof(from), ref from);
            src.Get(nameof(type), ref type);
            src.Get(nameof(text), ref text);
            src.Get(nameof(time), ref time);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(fromid), fromid);
            snk.Put(nameof(from), from);
            snk.Put(nameof(type), type);
            snk.Put(nameof(text), text);
            snk.Put(nameof(time), time);
        }
    }
}