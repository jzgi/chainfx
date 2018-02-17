using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// A chatting conversation.
    /// 
    public class Chat : IData
    {
        public static readonly Chat Empty = new Chat();

        internal string orgid;
        internal string wx;
        internal ChatMsg[] msgs;
        internal DateTime quested;

        public void Read(ISource i, byte proj = 0x0f)
        {
            i.Get(nameof(orgid), ref orgid);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(quested), ref quested);
            i.Get(nameof(msgs), ref msgs);
        }

        public const int NUM = 6;

        public void Write(ISink o, byte proj = 0x0f)
        {
            if (msgs != null && msgs.Length > 0)
            {
                int start = msgs.Length - NUM;
                if (start < 0) start = 0;
                for (int i = start; i < msgs.Length; i++)
                {
                    ChatMsg msg = msgs[i];
                    o.Put(nameof(msg.name), msg.text);
                }
            }
        }
    }

    public struct ChatMsg : IData
    {
        internal string name;

        internal string text;

        public void Read(ISource i, byte proj = 0x0f)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(text), ref text);
        }

        public void Write(ISink o, byte proj = 0x0f)
        {
            o.Put(nameof(name), name);
            o.Put(nameof(text), text);
        }
    }
}