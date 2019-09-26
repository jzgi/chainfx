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
        static readonly Bucket[] buckets =
        {
            new Bucket(1024 * 4, factor * 32),
            new Bucket(1024 * 16, factor * 16),
            new Bucket(1024 * 64, factor * 16),
            new Bucket(1024 * 256, factor * 8),
            new Bucket(1024 * 1024, factor * 8)
        };

        public static byte[] Rent(int demand)
        {
            // locate the queue
            for (int i = 0; i < buckets.Length; i++)
            {
                var buck = buckets[i];
                if (buck.Spec < demand) continue;
                if (!buck.TryPop(out var buf))
                {
                    buf = new byte[buck.Spec];
                }
                return buf;
            }

            // out of pool scope
            return new byte[demand];
        }

        public static void Return(byte[] buf)
        {
            int len = buf.Length;
            for (int i = 0; i < buckets.Length; i++)
            {
                var buck = buckets[i];
                if (buck.Spec == len) // the right queue to add
                {
                    if (buck.Count < buck.Limit)
                    {
                        buck.Push(buf);
                    }
                }
                else if (buck.Spec > len)
                {
                    break;
                }
            }
        }

        class Bucket : ConcurrentStack<byte[]>
        {
            // buffer size in bytes
            readonly int spec;

            readonly int limit;

            internal Bucket(int spec, int limit)
            {
                this.spec = spec;
                this.limit = limit;
            }

            internal int Spec => spec;

            internal int Limit => limit;
        }
    }
}