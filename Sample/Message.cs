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

        public void ReadData(IDataInput i, ushort flags = 0)
        {
            i.Get(nameof(fromid), ref fromid);
            i.Get(nameof(from), ref from);
            i.Get(nameof(type), ref type);
            i.Get(nameof(text), ref text);
            i.Get(nameof(time), ref time);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort flags = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(fromid), fromid);
            o.Put(nameof(from), from);
            o.Put(nameof(type), type);
            o.Put(nameof(text), text);
            o.Put(nameof(time), time);
        }
    }
}