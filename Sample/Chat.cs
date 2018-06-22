using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A chatting conversation.
    /// </summary>
    public class Chat : IData
    {
        public static readonly Chat Empty = new Chat();

        internal string orgid;
        internal int custid;
        internal string custname;
        internal string custwx;
        internal Msg[] msgs;
        internal DateTime quested;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(orgid), ref orgid);
            s.Get(nameof(custid), ref custid);
            s.Get(nameof(custname), ref custname);
            s.Get(nameof(custwx), ref custwx);
            s.Get(nameof(msgs), ref msgs);
            s.Get(nameof(quested), ref quested);
        }

        public const int NUM = 6;

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(orgid), orgid);
            s.Put(nameof(custid), custid);
            s.Put(nameof(custname), custname);
            s.Put(nameof(custwx), custwx);
            s.Put(nameof(msgs), msgs);
            s.Put(nameof(quested), quested);
        }
    }

    public struct Msg : IData
    {
        internal string name;
        internal string text;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(name), ref name);
            s.Get(nameof(text), ref text);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(name), name);
            s.Put(nameof(text), text);
        }
    }
}