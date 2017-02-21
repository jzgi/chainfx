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

        // for byte buffers
        static readonly Que<byte[]>[] BPool =
        {
            new Que<byte[]>(1024 * 4, Cores * 16),
            new Que<byte[]>(1024 * 16, Cores * 16),
            new Que<byte[]>(1024 * 64, Cores * 8),
            new Que<byte[]>(1024 * 256, Cores * 8),
            new Que<byte[]>(1024 * 1024, Cores * 4),
        };

        // for char buffers
        static readonly Que<char[]>[] CPool =
        {
            new Que<char[]>(1024 * 1, Cores * 8),
            new Que<char[]>(1024 * 4, Cores * 8),
            new Que<char[]>(1024 * 16, Cores * 4),
            new Que<char[]>(1024 * 64, Cores * 2)
        };

        public static byte[] ByteBuffer(int demand)
        {
            // locate the queue
            for (int i = 0; i < BPool.Length; i++)
            {
                Que<byte[]> que = BPool[i];
                if (que.Spec < demand) continue;
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
            int len = buf.Length;
            for (int i = 0; i < BPool.Length; i++)
            {
                Que<byte[]> que = BPool[i];
                if (que.Spec == len) // the right queue to add
                {
                    if (que.Count < que.Limit)
                    {
                        que.Enqueue(buf);
                    }
                }
                else if (que.Spec > len)
                {
                    break;
                }
            }
        }

        public static char[] CharBuffer(int demand)
        {
            // locate the queue
            for (int i = 0; i < CPool.Length; i++)
            {
                Que<char[]> que = CPool[i];
                if (que.Spec < demand) continue;
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
            int len = buf.Length;
            for (int i = 0; i < CPool.Length; i++)
            {
                Que<char[]> que = CPool[i];
                if (que.Spec == len) // the right queue to add
                {
                    if (que.Count < que.Limit)
                    {
                        que.Enqueue(buf);
                    }
                }
                else if (que.Spec > len)
                {
                    break;
                }
            }
        }

        public static bool Return(IContent content)
        {
            if (!content.Poolable) return false;

            if (content.Sendable) // is a byte buffer
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