using System;

namespace Greatbone.Core
{
    /// <summary>
    /// The data object of an event in the event queue.
    /// </summary>
    public class Event : IData
    {
        internal long id;

        internal string name;

        internal string shard;

        internal string arg;

        internal string type;

        internal ArraySegment<byte> body;

        internal DateTime time;

        public void Read(IDataInput i, byte proj = 0x0f)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(shard), ref shard);
            i.Get(nameof(arg), ref arg);
            i.Get(nameof(type), ref type);
            i.Get(nameof(body), ref body);
            i.Get(nameof(time), ref time);
        }

        public void Write<R>(IDataOutput<R> o, byte proj = 0x0f) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(shard), shard);
            o.Put(nameof(arg), arg);
            o.Put(nameof(type), type);
            o.Put(nameof(body), body);
            o.Put(nameof(time), time);
        }
    }
}