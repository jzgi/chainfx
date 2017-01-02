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

        static readonly Que<byte[]>[] BytesQues =
        {
            new Que<byte[]>(1024 * 4, Cores * 16),
            new Que<byte[]>(1024 * 16, Cores *16),
            new Que<byte[]>(1024 * 64, Cores * 8),
            new Que<byte[]>(1024 * 256, Cores * 8),
            new Que<byte[]>(1024 * 1024, Cores * 4),
        };

        static readonly Que<char[]>[] CharsQues =
        {
            new Que<char[]>(1024 * 1, Cores * 8),
            new Que<char[]>(1024 * 4, Cores * 8),
            new Que<char[]>(1024 * 16, Cores * 4),
            new Que<char[]>(1024 * 64, Cores * 2)
        };


        public static byte[] BorrowByteBuf(int demand)
        {
            // locate the appropriate queue
            int i = 0;
            while (BytesQues[i].Spec < demand) i++;

            if (i < BytesQues.Length)
            {
                Que<byte[]> que = BytesQues[i];
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
            for (int i = 0; i < BytesQues.Length; i++)
            {
                Que<byte[]> q = BytesQues[i];
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
            while (CharsQues[i].Spec < demand) i++;

            if (i < CharsQues.Length)
            {
                Que<char[]> que = CharsQues[i];
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
            for (int i = 0; i < BytesQues.Length; i++)
            {
                Que<char[]> q = CharsQues[i];
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
            if (cont.Senable)
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