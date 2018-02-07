using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A lesson slide data object.
    /// </summary>
    public class Slide : IData
    {
        public static readonly Slide Empty = new Slide();

        internal short id;
        internal string lesson; // 01 lesson-name
        internal string title;
        internal string text; //
        internal string svg; //
        internal string layout;
        internal DateTime revised;

        public void Read(IDataInput i, byte proj = 0x1f)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(lesson), ref lesson);
            i.Get(nameof(title), ref title);
            i.Get(nameof(text), ref text);
            i.Get(nameof(svg), ref svg);
            i.Get(nameof(layout), ref layout);
            i.Get(nameof(revised), ref revised);
        }

        public void Write<R>(IDataOutput<R> o, byte proj = 0x1f) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(title), title);
            o.Put(nameof(lesson), lesson);
            o.Put(nameof(text), text);
            o.Put(nameof(svg), svg);
            o.Put(nameof(layout), layout);
            o.Put(nameof(revised), revised);
        }
    }
}