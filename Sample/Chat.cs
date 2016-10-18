using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>Represent a chat session.</summary>
    ///
    public struct Chat : IPersist
    {
        private int status;

        public string partner;

        private Message[] msgs;

        private long lasttime;

        WebContext wctx;

        internal void Put(string msg)
        {
            // msgs.Add(new Message());
        }

        public void Load(ISource sc, uint x = 0)
        {
            sc.Got(nameof(status), ref status);
            sc.Got(nameof(partner), ref partner);
            sc.Got(nameof(msgs), ref msgs);
            sc.Got(nameof(lasttime), ref lasttime);
        }

        public void Save<R>(ISink<R> sk, uint x = 0) where R : ISink<R>
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

        public void Load(ISource sc, uint x = 0)
        {
            throw new NotImplementedException();
        }

        public void Save<R>(ISink<R> sk, uint x = 0) where R : ISink<R>
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

        public void Load(ISource sc, uint x = 0)
        {
            sc.Got(nameof(id), ref id);
            sc.Got(nameof(subtype), ref subtype);
            sc.Got(nameof(@from), ref @from);
            sc.Got(nameof(to), ref to);
            sc.Got(nameof(content), ref content);
            sc.Got(nameof(time), ref time);
        }

        public void Save<R>(ISink<R> sk, uint x = 0) where R : ISink<R>
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