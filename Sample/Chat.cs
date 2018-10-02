using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A discussion thread data object.
    /// </summary>
    public class Chat : IData
    {
        public static readonly Chat Empty = new Chat();

        public const byte ID = 1, DETAIL = 2;

        internal int id;
        internal string subject;
        internal string uname;
        internal Post[] posts;
        internal DateTime posted;
        internal short fcount;
        internal string fname;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(subject), ref subject);
            s.Get(nameof(uname), ref uname);
            if ((proj & DETAIL) > 0)
            {
                s.Get(nameof(posts), ref posts);
            }
            s.Get(nameof(posted), ref posted);
            s.Get(nameof(fcount), ref fcount);
            s.Get(nameof(fname), ref fname);
            s.Get(nameof(status), ref status);
        }

        public const int NUM = 6;

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(subject), subject);
            s.Put(nameof(uname), uname);
            if ((proj & DETAIL) > 0)
            {
                s.Put(nameof(posts), posts);
            }
            s.Put(nameof(posted), posted);
            s.Put(nameof(fcount), fcount);
            s.Put(nameof(fname), fname);
            s.Put(nameof(status), status);
        }
    }

    public struct Post : IData
    {
        internal int uid;
        internal string uname;
        internal short teamat;
        internal string text;
        internal short img;
        internal DateTime time;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(teamat), ref teamat);
            s.Get(nameof(text), ref text);
            s.Get(nameof(img), ref img);
            s.Get(nameof(time), ref time);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(teamat), teamat);
            s.Put(nameof(text), text);
            s.Put(nameof(img), img);
            s.Put(nameof(time), time);
        }
    }
}