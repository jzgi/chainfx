using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace SkyChain
{
    /// <summary>
    /// A dynamically generated content in format of either bytes or chars. It always uses the buffer pool. 
    /// </summary>
    public abstract class DynamicContent : HttpContent, IContent
    {
        static readonly char[] DIGIT =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        static readonly char[] HEXU =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        // sexagesimal numbers
        static readonly string[] SEX =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"
        };

        protected static readonly short[] SHORT =
        {
            1,
            10,
            100,
            1000,
            10000
        };

        protected static readonly int[] INT =
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };

        protected static readonly long[] LONG =
        {
            1L,
            10L,
            100L,
            1000L,
            10000L,
            100000L,
            1000000L,
            10000000L,
            100000000L,
            1000000000L,
            10000000000L,
            100000000000L,
            1000000000000L,
            10000000000000L,
            100000000000000L,
            1000000000000000L,
            10000000000000000L,
            100000000000000000L,
            1000000000000000000L
        };

        // NOTE: HttpResponseStream doesn't have internal buffer
        byte[] bytebuf;

        // additional char buffer
        char[] charbuf;

        // number of bytes
        int count;

        // byte-wise etag checksum, for char-based output only
        ulong checksum;

        protected DynamicContent(bool octet, int capacity)
        {
            if (octet)
            {
                bytebuf = BorrowByteArray(capacity);
            }
            else
            {
                charbuf = BorrowCharArray(capacity);
            }
        }

        public abstract string Type { get; set; }

        public byte[] Buffer => bytebuf;

        public int Count => count;

        string etag;

        public string ETag => etag ??= TextUtility.ToHex(checksum);

        public void Clear()
        {
            if (bytebuf != null)
            {
                Return(bytebuf);
                bytebuf = null;
            }
            else
            {
                Return(charbuf);
                charbuf = null;
            }
            count = 0;
            checksum = 0;
        }

        void AddByte(byte b)
        {
            // ensure capacity
            int olen = bytebuf.Length; // old length
            if (count >= olen)
            {
                int nlen = olen * 2; // new doubled length
                byte[] old = bytebuf;
                bytebuf = BorrowByteArray(nlen);
                Array.Copy(old, 0, bytebuf, 0, olen);
                Return(old);
            }

            bytebuf[count++] = b;

            // calculate checksum
            ulong cs = checksum;
            cs ^= b; // XOR
            checksum = cs >> 57 | cs << 7; // circular left shift 7 bit
        }

        public void Add(char c)
        {
            if (bytebuf != null) // byte-oriented
            {
                // UTF-8 encoding but without surrogate support
                if (c < 0x80)
                {
                    // have at most seven bits
                    AddByte((byte) c);
                }
                else if (c < 0x800)
                {
                    // 2 char, 11 bits
                    AddByte((byte) (0xc0 | (c >> 6)));
                    AddByte((byte) (0x80 | (c & 0x3f)));
                }
                else
                {
                    // 3 char, 16 bits
                    AddByte((byte) (0xe0 | ((c >> 12))));
                    AddByte((byte) (0x80 | ((c >> 6) & 0x3f)));
                    AddByte((byte) (0x80 | (c & 0x3f)));
                }
            }
            else // char-oriented
            {
                // ensure capacity
                int olen = charbuf.Length; // old length
                if (count >= olen)
                {
                    int nlen = olen * 2; // new length
                    char[] old = charbuf;
                    charbuf = BorrowCharArray(nlen);
                    Array.Copy(old, 0, charbuf, 0, olen);
                    Return(old);
                }

                charbuf[count++] = c;
            }
        }

        public void Add(bool v)
        {
            Add(v ? "true" : "false");
        }

        internal void Add(byte[] v)
        {
            if (v == null) return;
            for (int i = 0; i < v.Length; i++)
            {
                Add(v[i]);
            }
        }

        public void Add(char[] v)
        {
            if (v == null) return;
            Add(v, 0, v.Length);
        }

        public void Add(char[] v, int offset, int len)
        {
            if (v == null) return;
            for (int i = offset; i < len; i++)
            {
                Add(v[i]);
            }
        }

        public void Add(string v)
        {
            if (v == null) return;
            Add(v, 0, v.Length);
        }

        public void Add(string v, int offset, int length)
        {
            if (v == null) return;
            int len = Math.Min(length, v.Length);
            for (int i = offset; i < len; i++)
            {
                Add(v[i]);
            }
        }

        public void Add(StringBuilder v)
        {
            if (v == null)
            {
                return;
            }
            Add(v, 0, v.Length);
        }

        public void Add(StringBuilder v, int offset, int len)
        {
            if (v == null)
            {
                return;
            }
            for (int i = offset; i < len; i++)
            {
                Add(v[i]);
            }
        }

        public void Add(sbyte v)
        {
            Add((short) v);
        }

        public void Add(byte v)
        {
            Add((short) v);
        }

        public void Add(short v)
        {
            if (v == 0)
            {
                Add('0');
                return;
            }

            int x = v; // convert to int
            if (v < 0)
            {
                Add('-');
                x = -x;
            }

            bool bgn = false;
            for (int i = SHORT.Length - 1; i > 0; i--)
            {
                int bas = SHORT[i];
                int q = x / bas;
                x = x % bas;
                if (q != 0 || bgn)
                {
                    Add(DIGIT[q]);
                    bgn = true;
                }
            }

            Add(DIGIT[x]); // last reminder
        }

        public void Add(int v)
        {
            if (v >= short.MinValue && v <= short.MaxValue)
            {
                Add((short) v);
                return;
            }

            if (v < 0)
            {
                Add('-');
                v = -v;
            }

            bool bgn = false;
            for (int i = INT.Length - 1; i > 0; i--)
            {
                int bas = INT[i];
                int q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Add(DIGIT[q]);
                    bgn = true;
                }
            }

            Add(DIGIT[v]); // last reminder
        }

        public void Add(long v)
        {
            if (v >= int.MinValue && v <= int.MaxValue)
            {
                Add((int) v);
                return;
            }

            if (v < 0)
            {
                Add('-');
                v = -v;
            }

            bool bgn = false;
            for (int i = LONG.Length - 1; i > 0; i--)
            {
                long bas = LONG[i];
                long q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Add(DIGIT[q]);
                    bgn = true;
                }
            }

            Add(DIGIT[v]); // last reminder
        }

        public void Add(double v)
        {
            Add(v.ToString(CultureInfo.CurrentCulture));
        }

        // sign mask
        const int Sign = unchecked((int) 0x80000000);

        ///
        /// This method outputs decimal numbers fastly.
        ///
        public void Add(decimal v)
        {
            int[] bits = decimal.GetBits(v); // get the binary representation
            int low = bits[0], mid = bits[1], hi = bits[2], flags = bits[3];
            int scale = (bits[3] >> 16) & 0x7F;

            if (hi != 0) // if 96 bits, use system api
            {
                Add(v.ToString(CultureInfo.CurrentCulture));
                return;
            }

            // output a minus if negative
            if ((flags & Sign) != 0)
            {
                Add('-');
            }

            if (mid != 0) // if 64 bits
            {
                long x = ((long) mid << 32) + low;
                bool bgn = false;
                for (int i = LONG.Length - 1; i > 0; i--)
                {
                    long bas = LONG[i];
                    long q = x / bas;
                    x = x % bas;
                    if (q != 0 || bgn)
                    {
                        Add(DIGIT[q]);
                        bgn = true;
                    }

                    if (i == scale)
                    {
                        if (!bgn)
                        {
                            Add('0'); // 0.XX
                            bgn = true;
                        }

                        Add('.');
                    }
                }

                Add(DIGIT[x]); // last reminder
            }
            else // 32 bits
            {
                int x = low;
                bool bgn = false;
                for (int i = INT.Length - 1; i > 0; i--)
                {
                    int bas = INT[i];
                    int q = x / bas;
                    x = x % bas;
                    if (q != 0 || bgn)
                    {
                        Add(DIGIT[q]);
                        bgn = true;
                    }

                    if (i == scale)
                    {
                        if (!bgn)
                        {
                            Add('0'); // 0.XX
                            bgn = true;
                        }

                        Add('.');
                    }
                }

                Add(DIGIT[x]); // last reminder
            }

            // to pad extra zeros for monetary output
            if (scale == 0)
            {
                Add(".00");
            }
            else if (scale == 1)
            {
                Add('0');
            }
        }

        public void Add(JNumber v)
        {
            Add(v.Long);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
        }

        public void Add(DateTime v, byte date = 3, byte time = 3)
        {
            if (date >= 3)
            {
                short yr = (short) v.Year;

                // yyyy-mm-dd
                if (yr < 1000) Add('0');
                if (yr < 100) Add('0');
                if (yr < 10) Add('0');
                Add(v.Year);
            }

            if (date > 2) Add('-');
            if (date >= 2) Add(SEX[v.Month]);
            if (date > 1) Add('-');
            if (date >= 1) Add(SEX[v.Day]);

            if (time >= 1)
            {
                Add(' '); // a space for separation
                Add(SEX[v.Hour]);
            }

            if (time >= 2)
            {
                Add(':');
                Add(SEX[v.Minute]);
            }

            if (time >= 3)
            {
                Add(':');
                Add(SEX[v.Second]);
            }
        }

        public void AddPrimitive<V>(V v)
        {
            if (v is short shortv) Add(shortv);
            else if (v is int intv) Add(intv);
            else if (v is long longv) Add(longv);
            else if (v is string strv) Add(strv);
            else if (v is bool boolv) Add(boolv);
            else if (v is decimal decv) Add(decv);
            else if (v is double doublev) Add(doublev);
            else if (v is DateTime dtv) Add(dtv);
            else
            {
                Add(v?.ToString());
            }
        }

        public string MD5(string appendix = null, bool uppercase = false)
        {
            if (appendix != null)
            {
                Add(appendix);
            }

            // upper or lower
            var arr = uppercase ? HEXU : HEX;
            // digest and transform
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(bytebuf, 0, count);
            var str = new StringBuilder(32);
            for (int i = 0; i < 16; i++)
            {
                byte b = hash[i];
                str.Append(arr[b >> 4]);
                str.Append(arr[b & 0x0f]);
            }

            return str.ToString();
        }


        //
        // CLIENT CONTENT
        //
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return stream.WriteAsync(bytebuf, 0, count);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = count;
            return true;
        }

        public override string ToString()
        {
            return charbuf == null ? Type : new string(charbuf, 0, count);
        }


        // we use number of processor cores as a factor
        static readonly int factor = (int) Math.Log(ProcessorCount, 2) + 1;

        // pool of bytearray
        static readonly Stack<byte>[] bapool =
        {
            new Stack<byte>(1024 * 16, factor * 8),
            new Stack<byte>(1024 * 32, factor * 8),
            new Stack<byte>(1024 * 64, factor * 4),
            new Stack<byte>(1024 * 128, factor * 4),
            new Stack<byte>(1024 * 256, factor * 2),
            new Stack<byte>(1024 * 512, factor * 2),
        };

        static byte[] BorrowByteArray(int size)
        {
            // seek the stack
            for (int i = 0; i < bapool.Length; i++)
            {
                var stack = bapool[i];
                if (stack.Spec < size)
                {
                    continue;
                }
                if (!stack.TryPop(out var buf))
                {
                    buf = new byte[stack.Spec];
                }
                return buf;
            }

            // out of pool scope
            return new byte[size];
        }

        static void Return(byte[] buf)
        {
            if (buf == null) return;

            int len = buf.Length;
            for (int i = 0; i < bapool.Length; i++)
            {
                var stack = bapool[i];
                if (stack.Spec == len) // the right stack
                {
                    if (stack.Count < stack.Capacity)
                    {
                        stack.Push(buf);
                    }
                }
                else if (stack.Spec > len)
                {
                    break;
                }
            }
        }

        //
        // pool of chararray
        static readonly Stack<char>[] capool =
        {
            new Stack<char>(1024 * 1, factor * 8),
            new Stack<char>(1024 * 2, factor * 8),
            new Stack<char>(1024 * 4, factor * 4),
            new Stack<char>(1024 * 8, factor * 4),
            new Stack<char>(1024 * 32, factor * 2),
            new Stack<char>(1024 * 64, factor * 2)
        };

        public static char[] BorrowCharArray(int size)
        {
            // seek the stack
            for (int i = 0; i < capool.Length; i++)
            {
                var stack = capool[i];
                if (stack.Spec < size)
                {
                    continue;
                }
                if (!stack.TryPop(out var buf))
                {
                    buf = new char[stack.Spec];
                }
                return buf;
            }

            // out of pool scope
            return new char[size];
        }

        public static void Return(char[] buf)
        {
            if (buf == null) return;

            int len = buf.Length;
            for (int i = 0; i < capool.Length; i++)
            {
                var stack = capool[i];
                if (stack.Spec == len) // the right stack
                {
                    if (stack.Count < stack.Capacity)
                    {
                        stack.Push(buf);
                    }
                }
                else if (stack.Spec > len)
                {
                    break;
                }
            }
        }

        class Stack<T> : ConcurrentStack<T[]> where T : struct
        {
            // buffer size in bytes
            readonly int spec;

            readonly int capacity;

            internal Stack(int spec, int capacity)
            {
                this.spec = spec;
                this.capacity = capacity;
            }

            internal int Spec => spec;

            internal int Capacity => capacity;
        }
    }
}