using System;
using System.Collections.Concurrent;

namespace Greatbone.Core
{
    ///
    /// A global byte/char buffer pool.
    ///
    public static class BufferUtility
    {
        static readonly int Cores = Environment.ProcessorCount;

        static readonly Queue<byte[]>[] BQueues =
        {
            new Queue<byte[]>(1024 * 4, Cores * 64),
            new Queue<byte[]>(1024 * 16, Cores * 32),
            new Queue<byte[]>(1024 * 64, Cores * 16),
            new Queue<byte[]>(1024 * 256, Cores * 8),
            new Queue<byte[]>(1024 * 1024, Cores * 4),
        };

        static readonly Queue<char[]>[] CQueues =
        {
            new Queue<char[]>(1024 * 1, Cores * 64),
            new Queue<char[]>(1024 * 4, Cores * 32),
            new Queue<char[]>(1024 * 16, Cores * 16),
            new Queue<char[]>(1024 * 64, Cores * 8),
            new Queue<char[]>(1024 * 256, Cores * 4),
        };


        public static byte[] GetByteBuf(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (BQueues[i].Spec < demand) i++;

            if (i < BQueues.Length)
            {
                Queue<byte[]> que = BQueues[i];
                byte[] buf;
                if (!que.TryDequeue(out buf))
                {
                    buf = new byte[que.Spec];
                }
                return buf;
            }
            else
            {
                return new byte[demand];
            }
        }

        public static void Return(byte[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < BQueues.Length; i++)
            {
                Queue<byte[]> q = BQueues[i];
                if (q.Spec == blen) // the right queue to add
                {
                    if (q.Count < q.Limit)
                    {
                        q.Enqueue(buf);
                    }
                }
                else if (q.Spec > blen)
                {
                    break;
                }
            }
        }

        public static char[] GetCharBuf(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (CQueues[i].Spec < demand) i++;

            if (i < CQueues.Length)
            {
                Queue<char[]> que = CQueues[i];
                char[] buf;
                if (!que.TryDequeue(out buf))
                {
                    buf = new char[que.Spec];
                }
                return buf;
            }
            else
            {
                return new char[demand];
            }
        }

        public static void Return(char[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < BQueues.Length; i++)
            {
                Queue<char[]> q = CQueues[i];
                if (q.Spec == blen) // the right queue to add
                {
                    if (q.Count < q.Limit)
                    {
                        q.Enqueue(buf);
                    }
                }
                else if (q.Spec > blen)
                {
                    break;
                }
            }
        }


        public static bool Return(IContent cont)
        {
            if (!cont.IsPooled)
            {
                return false;
            }
            if (cont.IsRaw)
            {
                Return(cont.ByteBuf);
            }
            else
            {
                Return(cont.CharBuf);
            }
            return true;
        }

        class Queue<T> : ConcurrentQueue<T>
        {
            readonly int limit;

            // buffer size in bytes
            readonly int spec;

            internal Queue(int spec, int limit)
            {
                this.spec = spec;
                this.limit = limit;
            }

            internal int Spec => spec;

            internal int Limit => limit;
        }
    }
}