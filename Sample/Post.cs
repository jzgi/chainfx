using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>A brand object.</summary>
    /// <example>
    ///     Brand o  new Brand(){}
    /// </example>
    public class Post : IPersist
    {
        internal int id;

        internal DateTime time;

        internal string authorid;

        internal string author;

        internal bool commentable;

        internal Comment[] comments;

        internal string text;

        internal byte[] m0, m1, m2, m3, m4, m5, m6, m7, m8, m9;
        internal char[] mbits;


        ///
        /// <summary>Returns the key of the brand object.</summary>
        public string Key { get; }

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(time), ref time);
            sc.Got(nameof(authorid), ref authorid);
            sc.Got(nameof(author), ref author);
            sc.Got(nameof(commentable), ref commentable);
            sc.Got(nameof(comments), ref comments);
            sc.Got(nameof(text), ref text);

            sc.Got(nameof(m0), ref m0);
            sc.Got(nameof(m1), ref m1);
            sc.Got(nameof(m2), ref m2);
            sc.Got(nameof(m3), ref m3);
            sc.Got(nameof(m4), ref m4);
            sc.Got(nameof(m5), ref m5);
            sc.Got(nameof(m6), ref m6);
            sc.Got(nameof(m7), ref m7);
            sc.Got(nameof(m8), ref m8);
            sc.Got(nameof(m9), ref m9);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(time), time);
            sk.Put(nameof(authorid), authorid);
            sk.Put(nameof(author), author);
            sk.Put(nameof(commentable), commentable);
            sk.Put(nameof(comments), comments);
            sk.Put(nameof(text), text);

            sk.Put(nameof(m0), m0);
            sk.Put(nameof(m1), m1);
            sk.Put(nameof(m2), m2);
            sk.Put(nameof(m3), m3);
            sk.Put(nameof(m4), m4);
            sk.Put(nameof(m5), m5);
            sk.Put(nameof(m6), m6);
            sk.Put(nameof(m7), m7);
            sk.Put(nameof(m8), m8);
            sk.Put(nameof(m9), m9);
        }
    }

    public struct Comment : IPersist
    {
        internal DateTime time;

        internal short emoji;

        internal string authorid;

        internal string author;

        internal string text;

        public void Load(ISource sc, ushort x = 0xffff)
        {
            sc.Got(nameof(time), ref time);
            sc.Got(nameof(emoji), ref emoji);
            sc.Got(nameof(authorid), ref authorid);
            sc.Got(nameof(author), ref author);
            sc.Got(nameof(text), ref text);
        }

        public void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>
        {
            sk.Put(nameof(time), time);
            sk.Put(nameof(emoji), emoji);
            sk.Put(nameof(authorid), authorid);
            sk.Put(nameof(author), author);
            sk.Put(nameof(text), text);
        }
    }
}