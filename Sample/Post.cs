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
    public class Post
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

        public void From(IInput r)
        {
            r.Get(nameof(id), ref id);
            r.Get(nameof(time), ref time);
            r.Get(nameof(authorid), ref authorid);
            r.Get(nameof(author), ref author);
            r.Get(nameof(commentable), ref commentable);
            r.Get(nameof(comments), ref comments);
            r.Get(nameof(text), ref text);

            r.Get(nameof(m0), ref m0);
            r.Get(nameof(m1), ref m1);
            r.Get(nameof(m2), ref m2);
            r.Get(nameof(m3), ref m3);
            r.Get(nameof(m4), ref m4);
            r.Get(nameof(m5), ref m5);
            r.Get(nameof(m6), ref m6);
            r.Get(nameof(m7), ref m7);
            r.Get(nameof(m8), ref m8);
            r.Get(nameof(m9), ref m9);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(id), id);
            w.Put(nameof(time), time);
            w.Put(nameof(authorid), authorid);
            w.Put(nameof(author), author);
            w.Put(nameof(commentable), commentable);
            w.Put(nameof(comments), comments);
            w.Put(nameof(text), text);

            w.Put(nameof(m0), m0);
            w.Put(nameof(m1), m1);
            w.Put(nameof(m2), m2);
            w.Put(nameof(m3), m3);
            w.Put(nameof(m4), m4);
            w.Put(nameof(m5), m5);
            w.Put(nameof(m6), m6);
            w.Put(nameof(m7), m7);
            w.Put(nameof(m8), m8);
            w.Put(nameof(m9), m9);
        }
    }

    public struct Comment
    {
        internal DateTime time;

        internal short emoji;

        internal string authorid;

        internal string author;

        internal string text;

        public void From(IInput r)
        {
            r.Get(nameof(time), ref time);
            r.Get(nameof(emoji), ref emoji);
            r.Get(nameof(authorid), ref authorid);
            r.Get(nameof(author), ref author);
            r.Get(nameof(text), ref text);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(time), time);
            w.Put(nameof(emoji), emoji);
            w.Put(nameof(authorid), authorid);
            w.Put(nameof(author), author);
            w.Put(nameof(text), text);
        }
    }
}