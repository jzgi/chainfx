using System;
using System.Collections.Concurrent;
using static System.Environment;

namespace CloudUn
{
    /// <summary>
    /// A global byte buffer pool.
    /// </summary>
    public static class BufferUtility
    {
        // we use number of processor cores as a factor
        static readonly int factor = (int) Math.Log(ProcessorCount, 2) + 1;

        // pool of byte buffers
        static readonly Pool[] pools =
        {
            new Pool(1024 * 8, factor * 16),
            new Pool(1024 * 16, factor * 16),
            new Pool(1024 * 32, factor * 16),
            new Pool(1024 * 64, factor * 8),
            new Pool(1024 * 128, factor * 8),
            new Pool(1024 * 256, factor * 8),
            new Pool(1024 * 512, factor * 4)
        };

        public static byte[] Rent(int demand)
        {
            // locate the queue
            for (int i = 0; i < pools.Length; i++)
            {
                var pool = pools[i];
                if (pool.Spec < demand) continue;
                if (!pool.TryPop(out var buf))
                {
                    buf = new byte[pool.Spec];
                }
                return buf;
            }

            // out of pool scope
            return new byte[demand];
        }

        public static void Return(byte[] buf)
        {
            if (buf == null) return;

            int len = buf.Length;
            for (int i = 0; i < pools.Length; i++)
            {
                var pool = pools[i];
                if (pool.Spec == len) // the right queue to add
                {
                    if (pool.Count < pool.Limit)
                    {
                        pool.Push(buf);
                    }
                }
                else if (pool.Spec > len)
                {
                    break;
                }
            }
        }

        class Pool : ConcurrentStack<byte[]>
        {
            // buffer size in bytes
            readonly int spec;

            readonly int limit;

            internal Pool(int spec, int limit)
            {
                this.spec = spec;
                this.limit = limit;
            }

            internal int Spec => spec;

            internal int Limit => limit;
        }
    }
}