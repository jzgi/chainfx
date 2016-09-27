using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>Represent a chat session.</summary>
    ///
    public struct Chat
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

        public void From(IInput r)
        {
            r.Get(nameof(status), ref status);
            r.Get(nameof(partner), ref partner);
            r.Get(nameof(msgs), ref msgs);
            r.Get(nameof(lasttime), ref lasttime);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(status), status);
            w.Put(nameof(partner), partner);
            w.Put(nameof(msgs), msgs);
            w.Put(nameof(lasttime), lasttime);
        }
    }

    struct Message
    {
        DateTime time;

        string text;
    }

    public struct Msg
    {
        internal int id;

        internal short subtype;

        internal string from;

        internal string to;

        internal string content;

        internal DateTime time;

        public void From(IInput r)
        {
            r.Get(nameof(id), ref id);
            r.Get(nameof(subtype), ref subtype);
            r.Get(nameof(@from), ref @from);
            r.Get(nameof(to), ref to);
            r.Get(nameof(content), ref content);
            r.Get(nameof(time), ref time);
        }

        public void To(IOutput w)
        {
            w.Put(nameof(id), id);
            w.Put(nameof(subtype), subtype);
            w.Put(nameof(from), from);
            w.Put(nameof(to), to);
            w.Put(nameof(content), content);
            w.Put(nameof(time), time);
        }
    }
}