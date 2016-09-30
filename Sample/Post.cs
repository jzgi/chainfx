using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>A brand object.</summary>
    /// <example>
    ///     Brand o  new Brand(){}
    /// </example>
    public class Post : IData
    {
        internal int id;

        internal DateTime time;

        internal string authorid;

        internal string author;

        internal bool commentable;

        internal List<Comment> comments;

        internal string text;

        internal byte[] m0, m1, m2, m3, m4, m5, m6, m7, m8, m9;
        internal char[] mbits;


        ///
        /// <summary>Returns the key of the brand object.</summary>
        public string Key { get; }

        public void In(IDataIn i)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(time), ref time);
            i.Get(nameof(authorid), ref authorid);
            i.Get(nameof(author), ref author);
            i.Get(nameof(commentable), ref commentable);
            i.Get(nameof(comments), ref comments);
            i.Get(nameof(text), ref text);

            i.Get(nameof(m0), ref m0);
            i.Get(nameof(m1), ref m1);
            i.Get(nameof(m2), ref m2);
            i.Get(nameof(m3), ref m3);
            i.Get(nameof(m4), ref m4);
            i.Get(nameof(m5), ref m5);
            i.Get(nameof(m6), ref m6);
            i.Get(nameof(m7), ref m7);
            i.Get(nameof(m8), ref m8);
            i.Get(nameof(m9), ref m9);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(time), time);
            o.Put(nameof(authorid), authorid);
            o.Put(nameof(author), author);
            o.Put(nameof(commentable), commentable);
            o.Put(nameof(comments), comments);
            o.Put(nameof(text), text);

            o.Put(nameof(m0), m0);
            o.Put(nameof(m1), m1);
            o.Put(nameof(m2), m2);
            o.Put(nameof(m3), m3);
            o.Put(nameof(m4), m4);
            o.Put(nameof(m5), m5);
            o.Put(nameof(m6), m6);
            o.Put(nameof(m7), m7);
            o.Put(nameof(m8), m8);
            o.Put(nameof(m9), m9);
        }
    }

    public struct Comment
    {
        internal DateTime time;

        internal short emoji;

        internal string authorid;

        internal string author;

        internal string text;

        public void From(IDataIn i)
        {
            i.Get(nameof(time), ref time);
            i.Get(nameof(emoji), ref emoji);
            i.Get(nameof(authorid), ref authorid);
            i.Get(nameof(author), ref author);
            i.Get(nameof(text), ref text);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(time), time);
            o.Put(nameof(emoji), emoji);
            o.Put(nameof(authorid), authorid);
            o.Put(nameof(author), author);
            o.Put(nameof(text), text);
        }
    }
}