using System;
using System.Collections.Concurrent;

namespace Greatbone.Core
{
    /// <summary>
    /// A global byte/char buffer pool.
    /// </summary>
    public static class BufferUtility
    {
        // we use number ocores as a factor
        static readonly int Factor = Environment.ProcessorCount <= 2 ? 1 : Environment.ProcessorCount <= 8 ? 2 : 4;

        // for byte buffers, remember that caching may exclusively hold some byte buffers
        static readonly Que<byte[]>[] BPool =
        {
            new Que<byte[]>(1024 * 4, Factor * 8),
            new Que<byte[]>(1024 * 8, Factor * 8),
            new Que<byte[]>(1024 * 16, Factor * 8),
            new Que<byte[]>(1024 * 32, Factor * 8),
            new Que<byte[]>(1024 * 64, Factor * 4),
            new Que<byte[]>(1024 * 128, Factor * 4),
            new Que<byte[]>(1024 * 256, Factor * 4),
            new Que<byte[]>(1024 * 512, Factor * 2),
            new Que<byte[]>(1024 * 1024, Factor * 2)
        };

        // for char buffers
        static readonly Que<char[]>[] CPool =
        {
            new Que<char[]>(1024 * 1, Factor * 8),
            new Que<char[]>(1024 * 4, Factor * 8),
            new Que<char[]>(1024 * 8, Factor * 8),
            new Que<char[]>(1024 * 16, Factor * 4),
            new Que<char[]>(1024 * 32, Factor * 4),
            new Que<char[]>(1024 * 64, Factor * 2)
        };

        public static byte[] GetByteBuffer(int demand)
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

        public static char[] GetCharBuffer(int demand)
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

        public static bool Return(DynamicContent dcont)
        {
            if (dcont.Octet) // is a byte buffer
            {
                Return(dcont.ByteBuffer);
            }
            else
            {
                Return(dcont.CharBuffer);
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