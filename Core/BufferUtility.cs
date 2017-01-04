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

        static readonly Que<byte[]>[] BytesQues =
        {
            new Que<byte[]>(1024 * 4, Cores * 16),
            new Que<byte[]>(1024 * 16, Cores * 16),
            new Que<byte[]>(1024 * 64, Cores * 8),
            new Que<byte[]>(1024 * 256, Cores * 8),
            new Que<byte[]>(1024 * 1024, Cores * 4),
        };

        static readonly Que<char[]>[] CharsQues =
        {
            new Que<char[]>(1024 * 1, Cores * 8),
            new Que<char[]>(1024 * 4, Cores * 8),
            new Que<char[]>(1024 * 16, Cores * 4),
            new Que<char[]>(1024 * 64, Cores * 2)
        };

        public static byte[] ByteBuffer(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (BytesQues[i].Spec < demand) i++;

            if (i < BytesQues.Length)
            {
                Que<byte[]> que = BytesQues[i];
                byte[] buf;
                if (!que.TryDequeue(out buf))
                {
                    buf = new byte[que.Spec];
                }
                return buf;
            }
            // out of queues scope
            return new byte[demand];
        }

        public static void Return(byte[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < BytesQues.Length; i++)
            {
                Que<byte[]> que = BytesQues[i];
                if (que.Spec == blen) // the right queue to add
                {
                    if (que.Count < que.Limit)
                    {
                        que.Enqueue(buf);
                    }
                }
                else if (que.Spec > blen)
                {
                    break;
                }
            }
        }

        public static char[] CharBuffer(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (CharsQues[i].Spec < demand) i++;

            if (i < CharsQues.Length)
            {
                Que<char[]> que = CharsQues[i];
                char[] buf;
                if (!que.TryDequeue(out buf))
                {
                    buf = new char[que.Spec];
                }
                return buf;
            }
            // out of queues scope
            return new char[demand];
        }

        public static void Return(char[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < CharsQues.Length; i++)
            {
                Que<char[]> que = CharsQues[i];
                if (que.Spec == blen) // the right queue to add
                {
                    if (que.Count < que.Limit)
                    {
                        que.Enqueue(buf);
                    }
                }
                else if (que.Spec > blen)
                {
                    break;
                }
            }
        }

        public static bool Return(IContent content)
        {
            if (!content.Poolable) return false;

            if (content.Senable) // is a byte buffer
            {
                Return(content.ByteBuffer);
            }
            else
            {
                Return(content.CharBuffer);
            }
            return true;
        }

        class Que<B> : ConcurrentQueue<B>
        {
            readonly int limit;

            // buffer size in bytes
            readonly int spec;

            internal Que(int spec, int limit)
            {
                this.spec = spec;
                this.limit = limit;
            }

            internal int Spec => spec;

            internal int Limit => limit;
        }
    }
}