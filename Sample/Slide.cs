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

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(lesson), ref lesson);
            s.Get(nameof(title), ref title);
            s.Get(nameof(text), ref text);
            s.Get(nameof(svg), ref svg);
            s.Get(nameof(layout), ref layout);
            s.Get(nameof(revised), ref revised);
        }

        public void Write<R>(ISink<R> s, byte proj = 0x0f) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(title), title);
            s.Put(nameof(lesson), lesson);
            s.Put(nameof(text), text);
            s.Put(nameof(svg), svg);
            s.Put(nameof(layout), layout);
            s.Put(nameof(revised), revised);
        }
    }
}