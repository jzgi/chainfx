using System;
using System.Collections.Concurrent;
using static System.Environment;

namespace Greatbone
{
    /// <summary>
    /// A global byte/char buffer pool.
    /// </summary>
    public static class BufferUtility
    {
        // we use number of processor cores as a factor
        static readonly int factor = (int) Math.Log(ProcessorCount, 2) + 1;

        // pool of byte buffers
        static readonly Queue<byte[]>[] bPool =
        {
            new Queue<byte[]>(1024 * 4, factor * 32),
            new Queue<byte[]>(1024 * 16, factor * 16),
            new Queue<byte[]>(1024 * 64, factor * 16),
            new Queue<byte[]>(1024 * 256, factor * 8),
            new Queue<byte[]>(1024 * 1024, factor * 8)
        };

        // pool of char buffers
        static readonly Queue<char[]>[] cPool =
        {
            new Queue<char[]>(1024 * 1, factor * 16),
            new Queue<char[]>(1024 * 4, factor * 8),
            new Queue<char[]>(1024 * 16, factor * 8),
            new Queue<char[]>(1024 * 64, factor * 4)
        };

        public static byte[] GetByteBuffer(int demand)
        {
            // locate the queue
            for (int i = 0; i < bPool.Length; i++)
            {
                var queue = bPool[i];
                if (queue.Spec < demand) continue;
                if (!queue.TryDequeue(out var buf))
                {
                    buf = new byte[queue.Spec];
                }

                return buf;
            }

            // out of queues scope
            return new byte[demand];
        }

        public static void Return(byte[] buf)
        {
            int len = buf.Length;
            for (int i = 0; i < bPool.Length; i++)
            {
                var queue = bPool[i];
                if (queue.Spec == len) // the right queue to add
                {
                    if (queue.Count < queue.Limit)
                    {
                        queue.Enqueue(buf);
                    }
                }
                else if (queue.Spec > len)
                {
                    break;
                }
            }
        }

        public static char[] GetCharBuffer(int demand)
        {
            // locate the queue
            for (int i = 0; i < cPool.Length; i++)
            {
                var queue = cPool[i];
                if (queue.Spec < demand) continue;
                if (!queue.TryDequeue(out var buf))
                {
                    buf = new char[queue.Spec];
                }

                return buf;
            }

            // out of queues scope
            return new char[demand];
        }

        public static void Return(char[] buf)
        {
            int len = buf.Length;
            for (int i = 0; i < cPool.Length; i++)
            {
                var queue = cPool[i];
                if (queue.Spec == len) // the right queue to add
                {
                    if (queue.Count < queue.Limit)
                    {
                        queue.Enqueue(buf);
                    }
                }
                else if (queue.Spec > len)
                {
                    break;
                }
            }
        }

        public static bool Return(DynamicContent dcont)
        {
            if (dcont.IsBinary) // is a byte buffer
            {
                Return(dcont.ByteBuffer);
            }
            else
            {
                Return(dcont.CharBuffer);
            }

            return true;
        }

        class Queue<B> : ConcurrentQueue<B>
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