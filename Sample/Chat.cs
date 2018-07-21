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
        internal string topic;
        internal int uid;
        internal string uname;
        internal string uwx;
        internal Msg[] msgs;
        internal short replies;
        internal DateTime posted;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(ctrid), ref ctrid);
            s.Get(nameof(topic), ref topic);
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(uwx), ref uwx);
            if ((proj & DETAIL) > 0)
            {
                s.Get(nameof(msgs), ref msgs);
            }
            s.Get(nameof(replies), ref replies);
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
            s.Put(nameof(topic), topic);
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(uwx), uwx);
            if ((proj & DETAIL) > 0)
            {
                s.Put(nameof(msgs), msgs);
            }
            s.Put(nameof(replies), replies);
            s.Put(nameof(posted), posted);
        }
    }

    public struct Msg : IData
    {
        internal int uid;
        internal string uname;
        internal string text;
        internal DateTime posted;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(text), ref text);
            s.Get(nameof(posted), ref posted);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(text), text);
            s.Put(nameof(posted), posted);
        }
    }
}