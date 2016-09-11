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
        }

        public void WriteTo(ISerialWriter r)
        {
            r.Write(nameof(id), id);
            r.Write(nameof(time), time);
            r.Write(nameof(authorid), authorid);
            r.Write(nameof(author), author);
            r.Write(nameof(commentable), commentable);
            r.Write(nameof(comments), comments);
            r.Write(nameof(text), text);
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