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

        internal List<Message> messages;

        internal int status;

        readonly SemaphoreSlim lck = new SemaphoreSlim(1, 1);

        volatile TaskCompletionSource<bool> tcs;

        internal async Task<Message[]> GetAsync(bool wait = false)
        {
            await lck.WaitAsync();
            try
            {
                if (messages == null || messages.Count == 0)
                {
                    if (wait)
                    {
                        tcs = new TaskCompletionSource<bool>();
                        bool arrival = await tcs.Task;
                    }
                    return null;
                }

                Message[] ret = messages.ToArray();
                messages.Clear();
                return ret;
            }
            finally
            {
                lck.Release();
            }
        }

        internal async Task Put(Message msg)
        {
            await lck.WaitAsync();
            try
            {
                if (messages == null)
                {
                    messages = new List<Message>(8);
                }
                messages.Add(msg);

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

        public void ReadData(IDataInput i, byte flags = 0)
        {
            i.Get(nameof(userwx), ref userwx);
            i.Get(nameof(messages), ref messages);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, byte flags = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(userwx), userwx);
            o.Put(nameof(messages), messages);
            o.Put(nameof(status), status);
        }
    }
}