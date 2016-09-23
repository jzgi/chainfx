using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>Represent a chat session.</summary>
    ///
    public struct Chat : ISerial
    {
        private int status;

        public string partner;

        private List<Message> msgs;

        private long lasttime;

        WebContext wctx;

        internal void Put(string msg)
        {
            msgs.Add(new Message());
        }

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(status), ref status);
            r.Read(nameof(partner), ref partner);
            r.Read(nameof(msgs), ref msgs);
            r.Read(nameof(lasttime), ref lasttime);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(status), status);
            w.Write(nameof(partner), partner);
            w.Write(nameof(msgs), msgs);
            w.Write(nameof(lasttime), lasttime);
        }
    }

    struct Message
    {
        DateTime time;

        string text;
    }

    public struct Msg : ISerial
    {
        internal int id;

        internal short subtype;

        internal string from;

        internal string to;

        internal string content;

        internal DateTime time;

        public void ReadFrom(ISerialReader r)
        {
            r.Read(nameof(id), ref id);
            r.Read(nameof(subtype), ref subtype);
            r.Read(nameof(@from), ref @from);
            r.Read(nameof(to), ref to);
            r.Read(nameof(content), ref content);
            r.Read(nameof(time), ref time);
        }

        public void WriteTo(ISerialWriter w)
        {
            w.Write(nameof(id), id);
            w.Write(nameof(subtype), subtype);
            w.Write(nameof(from), from);
            w.Write(nameof(to), to);
            w.Write(nameof(content), content);
            w.Write(nameof(time), time);
        }
    }
}