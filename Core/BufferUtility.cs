using System.Collections.Concurrent;
using static System.Environment;

namespace Greatbone.Core
{
    /// <summary>
    /// A global byte/char buffer pool.
    /// </summary>
    public static class BufferUtility
    {
        // we use number ocores as a factor
        static readonly int factor = ProcessorCount <= 2 ? 1 :
            ProcessorCount <= 4 ? 2 :
            ProcessorCount <= 8 ? 3 :
            ProcessorCount <= 16 ? 4 : 5;

        // for byte buffers, remember that caching may exclusively hold some byte buffers
        static readonly Queue<byte[]>[] bpool =
        {
            new Queue<byte[]>(1024 * 16, factor * 8),
            new Queue<byte[]>(1024 * 32, factor * 8),
            new Queue<byte[]>(1024 * 64, factor * 8),
            new Queue<byte[]>(1024 * 128, factor * 4),
            new Queue<byte[]>(1024 * 256, factor * 4),
            new Queue<byte[]>(1024 * 512, factor * 2),
            new Queue<byte[]>(1024 * 1024, factor * 2)
        };

        // for char buffers
        static readonly Queue<char[]>[] cpool =
        {
            new Queue<char[]>(1024 * 1, factor * 8),
            new Queue<char[]>(1024 * 2, factor * 8),
            new Queue<char[]>(1024 * 4, factor * 8),
            new Queue<char[]>(1024 * 8, factor * 8),
            new Queue<char[]>(1024 * 16, factor * 4),
            new Queue<char[]>(1024 * 32, factor * 4),
            new Queue<char[]>(1024 * 64, factor * 2)
        };

        public static byte[] GetByteBuffer(int demand)
        {
            // locate the queue
            for (int i = 0; i < bpool.Length; i++)
            {
                Queue<byte[]> queue = bpool[i];
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
            for (int i = 0; i < bpool.Length; i++)
            {
                Queue<byte[]> queue = bpool[i];
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
            for (int i = 0; i < cpool.Length; i++)
            {
                Queue<char[]> queue = cpool[i];
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
            for (int i = 0; i < cpool.Length; i++)
            {
                Queue<char[]> queue = cpool[i];
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