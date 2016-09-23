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
    public class Post : ISerial
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

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), ref id);
            r.Read(nameof(time), ref time);
            r.Read(nameof(authorid), ref authorid);
            r.Read(nameof(author), ref author);
            r.Read(nameof(commentable), ref commentable);
            r.Read(nameof(comments), ref comments);
            r.Read(nameof(text), ref text);

            r.Read(nameof(m0), ref m0);
            r.Read(nameof(m1), ref m1);
            r.Read(nameof(m2), ref m2);
            r.Read(nameof(m3), ref m3);
            r.Read(nameof(m4), ref m4);
            r.Read(nameof(m5), ref m5);
            r.Read(nameof(m6), ref m6);
            r.Read(nameof(m7), ref m7);
            r.Read(nameof(m8), ref m8);
            r.Read(nameof(m9), ref m9);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(id), id);
            w.Write(nameof(time), time);
            w.Write(nameof(authorid), authorid);
            w.Write(nameof(author), author);
            w.Write(nameof(commentable), commentable);
            w.Write(nameof(comments), comments);
            w.Write(nameof(text), text);

            w.Write(nameof(m0), m0);
            w.Write(nameof(m1), m1);
            w.Write(nameof(m2), m2);
            w.Write(nameof(m3), m3);
            w.Write(nameof(m4), m4);
            w.Write(nameof(m5), m5);
            w.Write(nameof(m6), m6);
            w.Write(nameof(m7), m7);
            w.Write(nameof(m8), m8);
            w.Write(nameof(m9), m9);
        }
    }

    public struct Comment : ISerial
    {
        internal DateTime time;

        internal short emoji;

        internal string authorid;

        internal string author;

        internal string text;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(time), ref time);
            r.Read(nameof(emoji), ref emoji);
            r.Read(nameof(authorid), ref authorid);
            r.Read(nameof(author), ref author);
            r.Read(nameof(text), ref text);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(time), time);
            w.Write(nameof(emoji), emoji);
            w.Write(nameof(authorid), authorid);
            w.Write(nameof(author), author);
            w.Write(nameof(text), text);
        }
    }
}