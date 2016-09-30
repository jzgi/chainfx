using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>Represent a chat session.</summary>
    ///
    public struct Chat : IData
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

        public void In(IDataIn i)
        {
            i.Get(nameof(status), ref status);
            i.Get(nameof(partner), ref partner);
            i.Get(nameof(msgs), ref msgs);
            i.Get(nameof(lasttime), ref lasttime);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(status), status);
            o.Put(nameof(partner), partner);
            o.Put(nameof(msgs), msgs);
            o.Put(nameof(lasttime), lasttime);
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

        public void In(IDataIn i)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(subtype), ref subtype);
            i.Get(nameof(@from), ref @from);
            i.Get(nameof(to), ref to);
            i.Get(nameof(content), ref content);
            i.Get(nameof(time), ref time);
        }

        public void Out<R>(IDataOut<R> o) where R : IDataOut<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(subtype), subtype);
            o.Put(nameof(from), from);
            o.Put(nameof(to), to);
            o.Put(nameof(content), content);
            o.Put(nameof(time), time);
        }
    }
}