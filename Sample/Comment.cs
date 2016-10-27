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

        public void Load(ISource s, byte x = 0xff)
        {
            s.Got(nameof(time), ref time);
            s.Got(nameof(emoji), ref emoji);
            s.Got(nameof(authorid), ref authorid);
            s.Got(nameof(author), ref author);
            s.Got(nameof(text), ref text);
        }

        public void Save<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
            s.Put(nameof(time), time);
            s.Put(nameof(emoji), emoji);
            s.Put(nameof(authorid), authorid);
            s.Put(nameof(author), author);
            s.Put(nameof(text), text);
        }
    }

}