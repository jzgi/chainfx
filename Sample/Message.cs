using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public struct Message : IData
    {
        internal int senderid;

        internal string sender;

        internal short subtype;

        internal string text;

        internal DateTime time;

        public void Load(ISource src, byte bits = 0)
        {
            src.Get(nameof(senderid), ref senderid);
            src.Get(nameof(sender), ref sender);
            src.Get(nameof(subtype), ref subtype);
            src.Get(nameof(text), ref text);
            src.Get(nameof(time), ref time);
        }

        public void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>
        {
            snk.Put(nameof(senderid), senderid);
            snk.Put(nameof(sender), sender);
            snk.Put(nameof(subtype), subtype);
            snk.Put(nameof(text), text);
            snk.Put(nameof(time), time);
        }
    }
}