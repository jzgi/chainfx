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

        static readonly Que<byte[]>[] BQues =
        {
            new Que<byte[]>(1024 * 4, Cores * 64),
            new Que<byte[]>(1024 * 16, Cores * 32),
            new Que<byte[]>(1024 * 64, Cores * 16),
            new Que<byte[]>(1024 * 256, Cores * 8),
            new Que<byte[]>(1024 * 1024, Cores * 4),
        };

        static readonly Que<char[]>[] CQues =
        {
            new Que<char[]>(1024 * 1, Cores * 64),
            new Que<char[]>(1024 * 4, Cores * 32),
            new Que<char[]>(1024 * 16, Cores * 16),
            new Que<char[]>(1024 * 64, Cores * 8),
            new Que<char[]>(1024 * 256, Cores * 4),
        };


        public static byte[] BorrowByteBuf(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (BQues[i].Spec < demand) i++;

            if (i < BQues.Length)
            {
                Que<byte[]> que = BQues[i];
                byte[] buf;
                if (!que.TryDequeue(out buf))
                {
                    buf = new byte[que.Spec];
                }
                return buf;
            }
            else
            {
                return new byte[demand];
            }
        }

        public static void Return(byte[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < BQues.Length; i++)
            {
                Que<byte[]> q = BQues[i];
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

        public static char[] BorrowCharBuf(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (CQues[i].Spec < demand) i++;

            if (i < CQues.Length)
            {
                Que<char[]> que = CQues[i];
                char[] buf;
                if (!que.TryDequeue(out buf))
                {
                    buf = new char[que.Spec];
                }
                return buf;
            }
            else
            {
                return new char[demand];
            }
        }

        public static void Return(char[] buf)
        {
            int blen = buf.Length;
            for (int i = 0; i < BQues.Length; i++)
            {
                Que<char[]> q = CQues[i];
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
            if (!cont.Poolable)
            {
                return false;
            }
            if (cont.Binary)
            {
                Return(cont.ByteBuffer);
            }
            else
            {
                Return(cont.CharBuffer);
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