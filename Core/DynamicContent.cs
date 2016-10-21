using System;
using System.Globalization;
using System.Text;

namespace Greatbone.Core
{
    ///
    /// A binary content that is dynamically generated, where strings are UTF-8 encoded.
    ///
    public abstract class DynamicContent : IContent
    {
        // hexidecimal characters
        static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        // possible chars for representing a number as a string
        static readonly byte[] DIGIT =
        {
            (byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7', (byte) '8', (byte) '9'
        };

        const byte Minus = (byte)'-';

        static readonly short[] SHORT =
        {
            1,
            10,
            100,
            1000,
            10000
        };

        static readonly int[] INT =
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

        static readonly long[] LONG =
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


        protected byte[] buffer; // NOTE: HttpResponseStream doesn't have internal buffer

        // number of bytes, not mutable in reading mode
        protected int count;

        // byte-wise etag checksum, for text-based output only
        ulong checksum;

        /// <summary>
        /// Creates a dynamic content in writing mode.
        /// </summary>
        /// <param name="capacity">The initial capacity of the content buffer.</param>
        protected DynamicContent(int capacity)
        {
            this.buffer = BufferPool.Borrow(capacity);
            this.count = 0;
        }


        public abstract string Type { get; }

        public byte[] Buffer => buffer;

        public int Length => count;

        public DateTime LastModified => default(DateTime);

        public ulong ETag => checksum;


        void Write(byte b)
        {
            // ensure capacity
            int olen = buffer.Length;
            if (count >= olen)
            {
                byte[] alloc = new byte[olen * 4];
                Array.Copy(buffer, 0, alloc, 0, olen);
                buffer = alloc;
            }
            // append
            buffer[count++] = b;

            // calculate checksum
            ulong cs = checksum;
            cs ^= b; // XOR
            checksum = cs >> 57 | cs << 7; // circular left shift 7 bit
        }

        public void Add(bool v)
        {
            Add(v ? "true" : "false");
        }

        public void Add(char c)
        {
            // UTF-8 encoding but without surrogate support
            if (c < 0x80)
            {
                // have at most seven bits
                Write((byte)c);
            }
            else if (c < 0x800)
            {
                // 2 char, 11 bits
                Write((byte)(0xc0 | (c >> 6)));
                Write((byte)(0x80 | (c & 0x3f)));
            }
            else
            {
                // 3 char, 16 bits
                Write((byte)(0xe0 | ((c >> 12))));
                Write((byte)(0x80 | ((c >> 6) & 0x3f)));
                Write((byte)(0x80 | (c & 0x3f)));
            }
        }

        public void Add(char[] v)
        {
            Add(v, 0, v.Length);
        }

        public void Add(char[] v, int offset, int len)
        {
            if (v != null)
            {
                for (int i = offset; i < len; i++)
                {
                    Add(v[i], false);
                }
            }
        }

        public void Add(string v)
        {
            Add(v, 0, v.Length);
        }

        public void Add(string v, int offset, int len)
        {
            if (v != null)
            {
                for (int i = offset; i < len; i++)
                {
                    Add(v[i]);
                }
            }
        }

        public void Add(StringBuilder v)
        {
            Add(v, 0, v.Length);
        }

        public void Add(StringBuilder v, int offset, int len)
        {
            if (v != null)
            {
                for (int i = offset; i < len; i++)
                {
                    Add(v[i], false);
                }
            }
        }

        public void Add(short v)
        {
            if (v == 0)
            {
                Write((byte)'0');
                return;
            }
            int x = v; // convert to int
            if (v < 0)
            {
                Write(Minus);
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
                    Write(DIGIT[q]);
                    bgn = true;
                }
            }
            Write(DIGIT[x]); // last reminder
        }

        public void Add(int v)
        {
            if (v >= short.MinValue && v <= short.MaxValue)
            {
                Add((short)v);
                return;
            }

            if (v < 0)
            {
                Write(Minus);
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
                    Write(DIGIT[q]);
                    bgn = true;
                }
            }
            Write(DIGIT[v]); // last reminder
        }

        public void Add(long v)
        {
            if (v >= int.MinValue && v <= int.MaxValue)
            {
                Add((int)v);
                return;
            }

            if (v < 0)
            {
                Write(Minus);
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
                    Write(DIGIT[q]);
                    bgn = true;
                }
            }
            Write(DIGIT[v]); // last reminder
        }

        public void Add(decimal v)
        {
            Add(v, true);
        }

        // sign mask
        private const int Sign = unchecked((int)0x80000000);

        public void Add(decimal dec, bool money)
        {
            if (money)
            {
                int[] bits = decimal.GetBits(dec); // get the binary representation
                int low = bits[0], mid = bits[1], flags = bits[3];

                if ((flags & Sign) != 0) // negative
                {
                    Add('-');
                }
                if (mid != 0) // money
                {
                    long x = (low & 0x00ffffff) + ((long)(byte)(low >> 24) << 24) + ((long)mid << 32);
                    bool bgn = false;
                    for (int i = LONG.Length - 1; i >= 2; i--)
                    {
                        long bas = INT[i];
                        long q = x / bas;
                        x = x % bas;
                        if (q != 0 || bgn)
                        {
                            Write(DIGIT[q]);
                            bgn = true;
                        }
                        if (i == 4)
                        {
                            if (!bgn)
                            {
                                Add('0');
                                bgn = true;
                            }
                            Add('.');
                        }
                    }
                }
                else // smallmoney
                {
                    int x = low;
                    bool bgn = false;
                    for (int i = INT.Length - 1; i >= 2; i--)
                    {
                        int bas = INT[i];
                        int q = x / bas;
                        x = x % bas;
                        if (q != 0 || bgn)
                        {
                            Write(DIGIT[q]);
                            bgn = true;
                        }
                        if (i == 4)
                        {
                            if (!bgn)
                            {
                                Add('0');
                                bgn = true;
                            }
                            Add('.');
                        }
                    }
                }
            }
            else // ordinal decimal number
            {
                Add(dec.ToString(NumberFormatInfo.CurrentInfo));
            }
        }

        // sexagesimal numbers
        static readonly string[] SEX = {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"
        };

        public void Add(DateTime v)
        {
            short yr = (short)v.Year;
            byte mon = (byte)v.Month,
            day = (byte)v.Day;

            Add(v.Year);
            Add('-');
            Add(SEX[v.Month]);
            Add('-');
            Add(SEX[v.Day]);

            int hr = v.Hour, min = v.Minute, sec = v.Second, mil = v.Millisecond;
            if (hr == 0 && min == 0 && sec == 0 && mil == 0) return;

            Add(' '); // a space for separation
            Add(SEX[hr]);
            Add(':');
            Add(SEX[min]);
            Add(':');
            Add(SEX[sec]);
        }

    }
}