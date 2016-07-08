using System;
using System.Globalization;
using System.Text;

namespace Greatbone.Core
{
    public abstract class Coder : IOut
    {
        // hexidecimal characters
        private static readonly char[] Hex =
            {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};

        // possible chars for representing a number as a string
        private static readonly byte[] Digits =
        {
            (byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7', (byte) '8',
            (byte) '9'
        };

        private const byte Minus = (byte) '-';

        private static readonly byte[] OfByte = {1, 10, 100};

        private static readonly short[] OfShort = {1, 10, 100, 1000, 10000};

        private static readonly int[] OfInt =
            {1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 1000000000};

        private static readonly long[] OfLong =
        {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 1000000000, 100000000000L,
            1000000000000L, 10000000000000L, 100000000000000L, 100000000000000L, 10000000000000000L, 100000000000000000L,
            1000000000000000000L
        };

        //
        // output buffer

        internal byte[] buffer; // NOTE: HttpResponseStream doesn't have internal buffer

        internal int offset;

        internal int count;

        private ulong _checksum; // byte-wise etag checksum

        internal Coder(int initial)
        {
            buffer = new byte[initial];
        }

        public abstract string ContentType { get; }
        public byte[] Buffer => buffer;
        public int Offset => offset;
        public int Count => count;

        private void Write(byte b)
        {
            // double the capacity as needed
            int len = buffer.Length;
            if (count == len)
            {
                byte[] old = buffer;
                buffer = new byte[len * 2];
                Array.Copy(old, buffer, len);
            }
            buffer[count++] = b; // append to the buffer

            // calculate into checksum
            ulong cs = _checksum;
            cs ^= b; // XOR
            _checksum = cs >> 57 | cs << 7; // circular left shift 7 bit
        }

        public void Put(bool v)
        {
            Write((byte) '1');
        }

        public void Put(byte v)
        {
            if (v == 0)
            {
                Put('0');
                return;
            }

            int x = v;
            bool bgn = false;
            for (int i = OfByte.Length - 1; i >= 0; i--)
            {
                int bas = OfByte[i];
                int q = x / bas;
                x %= bas;
                if (q != 0 || bgn)
                {
                    Write(Digits[q]);
                    bgn = true;
                }
            }
        }

        public void Put(short v)
        {
            if (v == 0)
            {
                Put('0');
                return;
            }

            int x = v;
            if (v < 0)
            {
                Write(Minus);
                x = -x;
            }
            bool bgn = false;
            for (int i = OfShort.Length - 1; i >= 0; i--)
            {
                int bas = OfShort[i];
                int q = x / bas;
                x = x % bas;
                if (q != 0 || bgn)
                {
                    Write(Digits[q]);
                    bgn = true;
                }
            }
        }

        public void Put(int v)
        {
            if (v == 0)
            {
                Put('0');
                return;
            }
            if (v < 0)
            {
                Write(Minus);
                v = -v;
            }
            bool bgn = false;
            for (int i = OfInt.Length - 1; i >= 0; i--)
            {
                int bas = OfInt[i];
                int q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Write(Digits[q]);
                    bgn = true;
                }
            }
        }

        public void Put(long v)
        {
            if (v == 0)
            {
                Put('0');
                return;
            }
            if (v < 0)
            {
                Write(Minus);
                v = -v;
            }
            bool bgn = false;
            for (int i = OfLong.Length - 1; i >= 0; i--)
            {
                long bas = OfLong[i];
                long q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Write(Digits[q]);
                    bgn = true;
                }
            }
        }

        public void Put(decimal v)
        {
            Put(v, true);
        }

        // sign mask
        private const int Sign = unchecked((int) 0x80000000);

        public void Put(decimal dec, bool money)
        {
            if (money)
            {
                int[] bits = decimal.GetBits(dec); // get the binary representation
                int low = bits[0], mid = bits[1], flags = bits[3];

                if ((flags & Sign) != 0) // negative
                {
                    Put('-');
                }

                if (mid != 0) // money
                {
                    long x = (low & 0x00ffffff) + ((long) (byte) (low >> 24) << 24) + ((long) mid << 32);
                    bool bgn = false;
                    for (int i = OfLong.Length - 1; i >= 2; i--)
                    {
                        long bas = OfInt[i];
                        long q = x / bas;
                        x = x % bas;
                        if (q != 0 || bgn)
                        {
                            Write(Digits[q]);
                            bgn = true;
                        }
                        if (i == 4)
                        {
                            if (!bgn)
                            {
                                Put('0');
                                bgn = true;
                            }
                            Put('.');
                        }
                    }
                }
                else // smallmoney
                {
                    int x = low;
                    bool bgn = false;
                    for (int i = OfInt.Length - 1; i >= 2; i--)
                    {
                        int bas = OfInt[i];
                        int q = x / bas;
                        x = x % bas;
                        if (q != 0 || bgn)
                        {
                            Write(Digits[q]);
                            bgn = true;
                        }
                        if (i == 4)
                        {
                            if (!bgn)
                            {
                                Put('0');
                                bgn = true;
                            }
                            Put('.');
                        }
                    }
                }
            }
            else // ordinal decimal number
            {
                Put(dec.ToString(NumberFormatInfo.CurrentInfo));
            }
        }

        public void Put(DateTime v)
        {
            Put(v, true);
        }

        public void Put(DateTime dt, bool time)
        {
            short yr = (short) dt.Year;
            byte mon = (byte) dt.Month, day = (byte) dt.Day;

            Put(yr);
            Put('-');
            if (mon < 10) Put('0');
            Write(mon);
            Put('-');
            if (day < 10) Put('0');
            Write(day);

            byte hr = (byte) dt.Hour, min = (byte) dt.Minute, sec = (byte) dt.Second;
            if (time)
            {
                Put(' '); // a space for separation
                if (hr < 10) Put('0');
                Write(hr);
                Put(':');
                if (min < 10) Put('0');
                Write(min);
                Put(':');
                if (sec < 10) Put('0');
                Write(sec);
            }
        }

        private void Put(char c)
        {
            // UTF-8 encoding but without surrogate support
            if (c < 0x80)
            {
                // have at most seven bits
                Write((byte) c);
            }
            else if (c < 0x800)
            {
                // 2 text, 11 bits
                Write((byte) (0xc0 | (c >> 6)));
                Write((byte) (0x80 | (c & 0x3f)));
            }
            else
            {
                // 3 text, 16 bits
                Write((byte) (0xe0 | ((c >> 12))));
                Write((byte) (0x80 | ((c >> 6) & 0x3f)));
                Write((byte) (0x80 | (c & 0x3f)));
            }
        }

        public void Put(string v, int offset, int len)
        {
            if (v != null)
            {
                for (int i = offset; i < len; i++)
                {
                    Put(v[i]);
                }
            }
        }

        public void Put(string v)
        {
            if (v != null)
            {
                for (int i = 0, len = v.Length; i < len; i++)
                {
                    Put(v[i]);
                }
            }
        }

        public void Put(StringBuilder v)
        {
            if (v != null)
            {
                for (int i = 0, len = v.Length; i < len; i++)
                {
                    Put(v[i]);
                }
            }
        }
    }
}