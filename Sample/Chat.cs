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

        public void Load(ISource sc)
        {
            sc.Got(nameof(status), out status);
            sc.Got(nameof(partner), out partner);
            sc.Got(nameof(msgs), out msgs);
            sc.Got(nameof(lasttime), out lasttime);
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            sk.Put(nameof(status), status);
            sk.Put(nameof(partner), partner);
            sk.Put(nameof(msgs), msgs);
            sk.Put(nameof(lasttime), lasttime);
        }
    }

    struct Message : IPersist
    {
        DateTime time;

        string text;

        public void Load(ISource sc)
        {
            throw new NotImplementedException();
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
        {
            throw new NotImplementedException();
        }
    }

    public struct Msg : IPersist
    {
        internal int id;

        internal short subtype;

        internal string from;

        internal string to;

        internal string content;

        internal DateTime time;

        public void Load(ISource sc)
        {
            sc.Got(nameof(id), out id);
            sc.Got(nameof(subtype), out subtype);
            sc.Got(nameof(@from), out @from);
            sc.Got(nameof(to), out to);
            sc.Got(nameof(content), out content);
            sc.Got(nameof(time), out time);
        }

        public void Save<R>(ISink<R> sk) where R : ISink<R>
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