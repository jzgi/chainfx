using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public class Chat : IData
    {
        public static readonly Chat Empty = new Chat();


        internal string shopid;

        internal string wx;

        internal DateTime quested;

        internal ChatMsg[] msgs;


        public void ReadData(IDataInput i, ushort proj = 255)
        {
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(quested), ref quested);
            i.Get(nameof(msgs), ref msgs);
        }

        public const int NUM = 6;

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 255) where R : IDataOutput<R>
        {
            if (msgs != null && msgs.Length > 0)
            {
                int count = msgs.Length;
                if (count > NUM)
                {
                    count = NUM;
                }
                for (int i = 0; i < count; i++)
                {
                    ChatMsg msg = msgs[i];
                    o.Put(nameof(msg.name), msg.text, msg.name);
                }
            }
        }
    }

    public struct ChatMsg : IData
    {
        internal string name;

        internal string text;

        public void ReadData(IDataInput i, ushort proj = 255)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(text), ref text);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 255) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(text), text);
        }
    }
}