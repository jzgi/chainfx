using System;
using Greatbone.Core;

namespace Greatbone.Samp
{
    /// <summary>
    /// A lesson data object.
    /// </summary>
    public class Lesson : IData
    {
        public static readonly Lesson Empty = new Lesson();

        internal string id;
        internal string name;
        internal string refid; // such as youku reference id
        internal DateTime modified;

        public void Read(IDataInput i, byte proj = 0x1f)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            i.Get(nameof(refid), ref refid);
            i.Get(nameof(modified), ref modified);
        }

        public void Write<R>(IDataOutput<R> o, byte proj = 0x1f) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            o.Put(nameof(refid), refid);
            o.Put(nameof(modified), modified);
        }
    }
}