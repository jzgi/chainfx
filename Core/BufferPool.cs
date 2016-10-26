using System;
using System.Collections.Concurrent;

namespace Greatbone.Core
{
    ///
    /// A globally accessible buffer pool that borrow out and returns back byte buffers of certain sizes.
    ///
    public static class BufferPool
    {
        static readonly int Cores = Environment.ProcessorCount;

        static readonly Queue[] Queues =
       {
            new Queue(1024 * 4, Cores * 64),
            new Queue(1024 * 16, Cores * 32),
            new Queue(1024 * 64, Cores * 16),
            new Queue(1024 * 256, Cores * 8),
            new Queue(1024 * 1024, Cores * 4),
        };

        static int Max = Queues[Queues.Length - 1].Spec;


        public static byte[] Borrow(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (Queues[i].Spec < demand)
            {
                i++;
            }
            Queue q = Queues[i];
            // get or create a buffer
            byte[] buf;
            if (!q.TryDequeue(out buf))
            {
                buf = new byte[q.Spec];
            }
            return buf;
        }

        public static void Return(byte[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < Queues.Length; i++)
            {
                Queue q = Queues[i];
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

        public static byte[] Expand(byte[] old)
        {
            int olen = old.Length;
            byte[] buf = Borrow(olen * 4);
            Array.Copy(old, buf, olen);
            Return(old);
            return buf;
        }

        internal class Queue : ConcurrentQueue<byte[]>
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