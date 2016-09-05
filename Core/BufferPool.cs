using System;
using System.Collections.Concurrent;

namespace Greatbone.Core
{
    ///
    /// A globally accessed buffer pool that leases out and returns back byte buffers.
    ///
    public static class BufferPool
    {
        private static readonly int Cores = Environment.ProcessorCount;

        private static readonly Queue[] Queues =
        {
            new Queue(1024 * 4, Cores * 64),
            new Queue(1024 * 16, Cores * 32),
            new Queue(1024 * 64, Cores * 16),
            new Queue(1024 * 256, Cores * 8),
            new Queue(1024 * 1024, Cores * 4),
            new Queue(1024 * 1024 * 4, Cores * 2),
        };

        static int Max = Queues[Queues.Length - 1].Spec;


        public static byte[] Lease(int demand)
        {
            //			byte[] result;
            //			if (!_queues[group].TryDequeue(out result))
            //			{
            //				result = new byte[_bufsize];
            //				_queues[group].Enqueue(result);
            //			}
            //			return result;
            return new byte[demand];
        }

        public static void Return(byte[] buf)
        {
            //			if (Interlocked.Increment(ref _count) < _capacity)
            //			{
            //				_queues.Enqueue(buf);
            //			}
        }

        public static byte[] Extend(byte[] old)
        {
            byte[] a = new byte[old.Length * 2];
            Array.Copy(old, a, old.Length);
            return a;
        }

        internal class Queue
        {
            private readonly int queued;

            // buffer size in bytes
            private readonly int spec;


            private readonly ConcurrentQueue<byte[]> queue;

            internal Queue(int spec, int queued)
            {
                this.spec = spec;
                this.queued = queued;
                queue = new ConcurrentQueue<byte[]>();
            }

            internal int Spec => spec;

            internal int Queued => queued;
        }
    }
}