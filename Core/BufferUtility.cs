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

        static readonly Queue<byte[]>[] bytebufs =
        {
            new Queue<byte[]>(1024 * 4, Cores * 64),
            new Queue<byte[]>(1024 * 16, Cores * 32),
            new Queue<byte[]>(1024 * 64, Cores * 16),
            new Queue<byte[]>(1024 * 256, Cores * 8),
            new Queue<byte[]>(1024 * 1024, Cores * 4),
        };

        static readonly Queue<char[]>[] charbufs =
        {
            new Queue<char[]>(1024 * 1, Cores * 64),
            new Queue<char[]>(1024 * 4, Cores * 32),
            new Queue<char[]>(1024 * 16, Cores * 16),
            new Queue<char[]>(1024 * 64, Cores * 8),
            new Queue<char[]>(1024 * 256, Cores * 4),
        };


        public static byte[] GetByteBuffer(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (bytebufs[i].Spec < demand)
            {
                i++;
            }
            Queue<byte[]> q = bytebufs[i];
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
            for (int i = 0; i < bytebufs.Length; i++)
            {
                Queue<byte[]> q = bytebufs[i];
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


        public static char[] GetCharBuffer(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (charbufs[i].Spec < demand)
            {
                i++;
            }
            Queue<char[]> q = charbufs[i];
            // get or create a buffer
            char[] buf;
            if (!q.TryDequeue(out buf))
            {
                buf = new char[q.Spec];
            }
            return buf;
        }

        public static void Return(char[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < bytebufs.Length; i++)
            {
                Queue<char[]> q = charbufs[i];
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
                Return(cont.ByteBuffer);
            }
            else
            {
                Return(cont.CharBuffer);
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