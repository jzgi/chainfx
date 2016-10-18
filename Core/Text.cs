using System;
using System.Globalization;
using System.Text;

namespace Greatbone.Core
{
    public class Text
    {

        static readonly char[] Digits =
        {
            '0',  '1',  '2',  '3',  '4',  '5',  '6',  '7',  '8',  '9'
        };

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

        const int InitialCapacity = 1024;

        internal char[] buffer;

        internal int count;

        protected Text(int capacity)
        {
            buffer = new char[capacity];
            count = 0;
        }

        public void Add(char c)
        {
            // ensure capacity
            int olen = buffer.Length;
            if (count >= olen)
            {
                char[] alloc = new char[olen * 4];
                Array.Copy(buffer, 0, alloc, 0, olen);
                buffer = alloc;
            }
            // append
            buffer[count++] = c;
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
            for (int i = Shorts.Length - 1; i > 0; i--)
            {
                int bas = Shorts[i];
                int q = x / bas;
                x = x % bas;
                if (q != 0 || bgn)
                {
                    Add(Digits[q]);
                    bgn = true;
                }
            }
            Add(Digits[x]); // last reminder
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
                Add('-');
                v = -v;
            }
            bool bgn = false;
            for (int i = Ints.Length - 1; i > 0; i--)
            {
                int bas = Ints[i];
                int q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Add(Digits[q]);
                    bgn = true;
                }
            }
            Add(Digits[v]); // last reminder
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
                Add('-');
                v = -v;
            }
            bool bgn = false;
            for (int i = Longs.Length - 1; i > 0; i--)
            {
                long bas = Longs[i];
                long q = v / bas;
                v = v % bas;
                if (q != 0 || bgn)
                {
                    Add(Digits[q]);
                    bgn = true;
                }
            }
            Add(Digits[v]); // last reminder
        }

        public void Add(decimal v)
        {
            Add(v, true);
        }

        public void Add(Number v)
        {
            Add(v.Long);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
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
                            Add(Digits[q]);
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
                            Add(Digits[q]);
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
            short mon = (byte)dt.Month, day = (byte)dt.Day;

            Add(yr);
            Add('-');
            if (mon < 10) Add('0');
            Add(mon);
            Add('-');
            if (day < 10) Add('0');
            Add(day);

            byte hr = (byte)dt.Hour, min = (byte)dt.Minute, sec = (byte)dt.Second;
            if (time)
            {
                Add(' '); // a space for separation
                if (hr < 10) Add('0');
                Add(hr);
                Add(':');
                if (min < 10) Add('0');
                Add(min);
                Add(':');
                if (sec < 10) Add('0');
                Add(sec);
            }
        }

        public override string ToString()
        {
            return new string(buffer, 0, count);
        }
    }
}