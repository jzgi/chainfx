using Greatbone.Core;
using System;

namespace Greatbone.Sample
{
    public struct Comment : IPersist
    {
        internal DateTime time;
        internal bool emoji;
        internal string authorid;
        internal string author;
        internal string text;

        public void Load(ISource s, byte x = 0)
        {
            s.Get(nameof(time), ref time);
            s.Get(nameof(emoji), ref emoji);
            s.Get(nameof(authorid), ref authorid);
            s.Get(nameof(author), ref author);
            s.Get(nameof(text), ref text);
        }

        public void Dump<R>(ISink<R> s, byte x = 0) where R : ISink<R>
        {
            s.Put(nameof(time), time);
            s.Put(nameof(emoji), emoji);
            s.Put(nameof(authorid), authorid);
            s.Put(nameof(author), author);
            s.Put(nameof(text), text);
        }

    }

}