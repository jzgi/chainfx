using System;
using Greatbone.Core;

namespace Greatbone.Samp
{
    /// <summary>
    /// A lesson data object. It supports Zh and En locales.
    /// </summary>
    public class Slide : IData
    {
        public static readonly Slide Empty = new Slide();

        internal string no;
        internal string lesson;
        internal string title;
        internal string text;
        internal string figure;
        internal DateTime modified;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(no), ref no);
            i.Get(nameof(lesson), ref lesson);
            i.Get(nameof(title), ref title);
            i.Get(nameof(text), ref text);
            i.Get(nameof(figure), ref figure);
            i.Get(nameof(modified), ref modified);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(no), no);
            o.Put(nameof(lesson), lesson);
            o.Put(nameof(title), title);
            o.Put(nameof(text), text);
            o.Put(nameof(figure), figure);
            o.Put(nameof(modified), modified);
        }
    }
}