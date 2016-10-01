using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>Represent a chat session.</summary>
    ///
    public struct Chat : IPersist
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

        public void Load(ISource sc, int x)
        {
            sc.Get(nameof(status), ref status);
            sc.Get(nameof(partner), ref partner);
            sc.Get(nameof(msgs), ref msgs);
            sc.Get(nameof(lasttime), ref lasttime);
        }

        public void Save<R>(ISink<R> sk, int x) where R : ISink<R>
        {
            sk.Put(nameof(status), status);
            sk.Put(nameof(partner), partner);
            sk.Put(nameof(msgs), msgs);
            sk.Put(nameof(lasttime), lasttime);
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

        public void Load(ISource sc, int x)
        {
            sc.Get(nameof(id), ref id);
            sc.Get(nameof(subtype), ref subtype);
            sc.Get(nameof(@from), ref @from);
            sc.Get(nameof(to), ref to);
            sc.Get(nameof(content), ref content);
            sc.Get(nameof(time), ref time);
        }

        public void Save<R>(ISink<R> sk, int x) where R : ISink<R>
        {
            sk.Put(nameof(id), id);
            sk.Put(nameof(subtype), subtype);
            sk.Put(nameof(from), from);
            sk.Put(nameof(to), to);
            sk.Put(nameof(content), content);
            sk.Put(nameof(time), time);
        }
    }
}