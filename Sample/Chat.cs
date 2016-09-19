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
            r.Read(nameof(status), out status);
            r.Read(nameof(partner), out partner);
            r.Read(nameof(msgs), out msgs);
            r.Read(nameof(lasttime), out lasttime);
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
            r.Read(nameof(id), out id);
            r.Read(nameof(subtype), out subtype);
            r.Read(nameof(@from), out @from);
            r.Read(nameof(to), out to);
            r.Read(nameof(content), out content);
            r.Read(nameof(time), out time);
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