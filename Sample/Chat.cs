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
        internal string ctrid;
        internal string subject;
        internal int uid;
        internal string uname;
        internal Msg[] msgs;
        internal short replies;
        internal short imgs;
        internal DateTime posted;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(ctrid), ref ctrid);
            s.Get(nameof(subject), ref subject);
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            if ((proj & DETAIL) > 0)
            {
                s.Get(nameof(msgs), ref msgs);
            }
            s.Get(nameof(replies), ref replies);
            s.Get(nameof(imgs), ref imgs);
            s.Get(nameof(posted), ref posted);
        }

        public const int NUM = 6;

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(ctrid), ctrid);
            s.Put(nameof(subject), subject);
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            if ((proj & DETAIL) > 0)
            {
                s.Put(nameof(msgs), msgs);
            }
            s.Put(nameof(replies), replies);
            s.Put(nameof(imgs), imgs);
            s.Put(nameof(posted), posted);
        }
    }

    public struct Msg : IData
    {
        internal int uid;
        internal string uname;
        internal string text;
        internal short img;
        internal DateTime posted;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(text), ref text);
            s.Get(nameof(img), ref img);
            s.Get(nameof(posted), ref posted);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(text), text);
            s.Put(nameof(img), img);
            s.Put(nameof(posted), posted);
        }
    }
}