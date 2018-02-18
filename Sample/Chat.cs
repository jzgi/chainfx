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

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(orgid), ref orgid);
            s.Get(nameof(wx), ref wx);
            s.Get(nameof(quested), ref quested);
            s.Get(nameof(msgs), ref msgs);
        }

        public const int NUM = 6;

        public void Write(ISink s, byte proj = 0x0f)
        {
            if (msgs != null && msgs.Length > 0)
            {
                int start = msgs.Length - NUM;
                if (start < 0) start = 0;
                for (int i = start; i < msgs.Length; i++)
                {
                    ChatMsg msg = msgs[i];
                    s.Put(nameof(msg.name), msg.text);
                }
            }
        }
    }

    public struct ChatMsg : IData
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