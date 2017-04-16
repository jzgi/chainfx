using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An in-memory cache of personal chatting logs.
    ///
    public class Chat : IData
    {
        internal string userwx; // openid of the owner

        internal List<ChatMsg> msgs;

        internal int status;

        readonly SemaphoreSlim lck = new SemaphoreSlim(1, 1);

        volatile TaskCompletionSource<bool> tcs;

        internal async Task<ChatMsg[]> GetAsync(bool wait = false)
        {
            await lck.WaitAsync();
            try
            {
                if (msgs == null || msgs.Count == 0)
                {
                    if (wait)
                    {
                        tcs = new TaskCompletionSource<bool>();
                        bool arrival = await tcs.Task;
                    }
                    return null;
                }

                ChatMsg[] ret = msgs.ToArray();
                msgs.Clear();
                return ret;
            }
            finally
            {
                lck.Release();
            }
        }

        internal async Task Put(ChatMsg msg)
        {
            await lck.WaitAsync();
            try
            {
                if (msgs == null)
                {
                    msgs = new List<ChatMsg>(8);
                }
                msgs.Add(msg);

                if (tcs != null)
                {
                    // try release a long polling
                    tcs.TrySetResult(true);
                    tcs = null;
                }
            }
            finally
            {
                lck.Release();
            }
        }

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(userwx), ref userwx);
            i.Get(nameof(msgs), ref msgs);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(userwx), userwx);
            o.Put(nameof(msgs), msgs);
            o.Put(nameof(status), status);
        }
    }


    public struct ChatMsg : IData
    {
        internal string fromid;

        internal string from;

        internal short type;

        internal string text;

        internal DateTime time;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(fromid), ref fromid);
            i.Get(nameof(from), ref from);
            i.Get(nameof(type), ref type);
            i.Get(nameof(text), ref text);
            i.Get(nameof(time), ref time);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(fromid), fromid);
            o.Put(nameof(@from), @from);
            o.Put(nameof(type), type);
            o.Put(nameof(text), text);
            o.Put(nameof(time), time);
        }
    }
}