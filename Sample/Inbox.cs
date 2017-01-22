using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An in-memory message inbox for a particular user.
    ///
    public class Inbox : IData
    {
        internal string userid;

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

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(userid), ref userid);
            src.Get(nameof(messages), ref messages);
            src.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(userid), userid);
            snk.Put(nameof(messages), messages);
            snk.Put(nameof(status), status);
        }
    }
}