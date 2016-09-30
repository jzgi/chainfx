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
        static readonly char[] Hex =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        // possible chars for representing a number as a string
        static readonly byte[] Digits =
        {
            (byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7', (byte) '8', (byte) '9'
        };

        const byte Minus = (byte)'-';

        static readonly short[] Shorts =
        {
            1,
            10,
            100,
            1000,
            10000
        };

        static readonly int[] Ints =
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

        static readonly long[] Longs =
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
        long checksum;

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

        public long ETag => checksum;


        public void AddByte(byte b)
        {
            // grow the capacity as needed
            int len = buffer.Length;
            if (count >= len)
            {
                byte[] old = buffer;
                buffer = new byte[len * 4];
                Array.Copy(old, buffer, len);
            }
            buffer[count++] = b; // append to the buffer

            // calculate checksum
            long cs = checksum;
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
                AddByte((byte)c);
            }
            else if (c < 0x800)
            {
                // 2 text, 11 bits
                AddByte((byte)(0xc0 | (c >> 6)));
                AddByte((byte)(0x80 | (c & 0x3f)));
            }
            else
            {
                // 3 text, 16 bits
                AddByte((byte)(0xe0 | ((c >> 12))));
                AddByte((byte)(0x80 | ((c >> 6) & 0x3f)));
                AddByte((byte)(0x80 | (c & 0x3f)));
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
                    Add(v[i]);
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
                    Add(v[i]);
                }
            }
        }

        public void Add(short v)
        {
            if (v == 0)
            {
                Add('0');
                return;
            }
            int x = v;
            if (v < 0)
            {
                AddByte(Minus);
                x = -x;
            }
            bool bgn = false;
            for (int i = Shorts.Length - 1; i >= 0; i--)
            {
                int bas = Shorts[i];
                int q = x / bas;
                x = x % bas;
                if (q != 0 || bgn)
                {
                    AddByte(Digits[q]);
                    bgn = true;
                }
            }
            AddByte(Digits[v]); // last reminder
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
                AddByte(Minus);
                v = -v;
            }
            bool bgn = false;
            for (int i = Ints.Length - 1; i >= 0; i--)
            {
                int bas = Ints[i];
                int q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    AddByte(Digits[q]);
                    bgn = true;
                }
            }
            AddByte(Digits[v]); // last reminder
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
                AddByte(Minus);
                v = -v;
            }
            bool bgn = false;
            for (int i = Longs.Length - 1; i >= 0; i--)
            {
                long bas = Longs[i];
                long q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    AddByte(Digits[q]);
                    bgn = true;
                }
            }
            AddByte(Digits[v]); // last reminder
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
                    for (int i = Longs.Length - 1; i >= 2; i--)
                    {
                        long bas = Ints[i];
                        long q = x / bas;
                        x = x % bas;
                        if (q != 0 || bgn)
                        {
                            AddByte(Digits[q]);
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
                    for (int i = Ints.Length - 1; i >= 2; i--)
                    {
                        int bas = Ints[i];
                        int q = x / bas;
                        x = x % bas;
                        if (q != 0 || bgn)
                        {
                            AddByte(Digits[q]);
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

        public void Add(DateTime v)
        {
            Add(v, true);
        }

        public void Add(DateTime dt, bool time)
        {
            short yr = (short)dt.Year;
            byte mon = (byte)dt.Month, day = (byte)dt.Day;

            Add(yr);
            Add('-');
            if (mon < 10) Add('0');
            AddByte(mon);
            Add('-');
            if (day < 10) Add('0');
            AddByte(day);

            byte hr = (byte)dt.Hour, min = (byte)dt.Minute, sec = (byte)dt.Second;
            if (time)
            {
                Add(' '); // a space for separation
                if (hr < 10) Add('0');
                AddByte(hr);
                Add(':');
                if (min < 10) Add('0');
                AddByte(min);
                Add(':');
                if (sec < 10) Add('0');
                AddByte(sec);
            }
        }
    }
}