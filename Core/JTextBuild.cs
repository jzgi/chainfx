using System;
using System.Globalization;

namespace Greatbone.Core
{
    public class JTextBuild : ISink<JTextBuild>
    {
        // possible chars for representing a number as a string
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

        char[] buffer;

        int count;

        // parsing context for levels
        int[] counts = new int[8];

        int level;

        public JTextBuild(int capacity = InitialCapacity)
        {
            buffer = new char[capacity];
            level = 0;
        }

        void Write(char c)
        {
            // grow the capacity as needed
            int len = buffer.Length;
            if (count >= len)
            {
                char[] old = buffer;
                buffer = new char[len * 4];
                Array.Copy(old, buffer, len);
            }
            buffer[count++] = c; // append to the buffer
        }

        void Add(char[] v)
        {
            Add(v, 0, v.Length);
        }

        void Add(char[] v, int offset, int len)
        {
            if (v != null)
            {
                for (int i = offset; i < len; i++)
                {
                    Write(v[i]);
                }
            }
        }

        void Add(string v)
        {
            Add(v, 0, v.Length);
        }

        void Add(string v, int offset, int len)
        {
            if (v != null)
            {
                for (int i = offset; i < len; i++)
                {
                    Write(v[i]);
                }
            }
        }

        void Add(short v)
        {
            if (v == 0)
            {
                Write('0');
                return;
            }
            int x = v;
            if (v < 0)
            {
                Write('-');
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
                    Write(Digits[q]);
                    bgn = true;
                }
            }
            Write(Digits[v]); // last reminder
        }

        void Add(int v)
        {
            if (v >= short.MinValue && v <= short.MaxValue)
            {
                Add((short)v);
                return;
            }

            if (v < 0)
            {
                Write('-');
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
                    Write(Digits[q]);
                    bgn = true;
                }
            }
            Write(Digits[v]); // last reminder
        }

        void Add(long v)
        {
            if (v >= int.MinValue && v <= int.MaxValue)
            {
                Add((int)v);
                return;
            }

            if (v < 0)
            {
                Write('-');
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
                    Write(Digits[q]);
                    bgn = true;
                }
            }
            Write(Digits[v]); // last reminder
        }

        void Add(decimal v)
        {
            Add(v, true);
        }

        // sign mask
        private const int Sign = unchecked((int)0x80000000);

        void Add(decimal dec, bool money)
        {
            if (money)
            {
                int[] bits = decimal.GetBits(dec); // get the binary representation
                int low = bits[0], mid = bits[1], flags = bits[3];

                if ((flags & Sign) != 0) // negative
                {
                    Write('-');
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
                            Write(Digits[q]);
                            bgn = true;
                        }
                        if (i == 4)
                        {
                            if (!bgn)
                            {
                                Write('0');
                                bgn = true;
                            }
                            Write('.');
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
                            Write(Digits[q]);
                            bgn = true;
                        }
                        if (i == 4)
                        {
                            if (!bgn)
                            {
                                Write('0');
                                bgn = true;
                            }
                            Write('.');
                        }
                    }
                }
            }
            else // ordinal decimal number
            {
                Add(dec.ToString(NumberFormatInfo.CurrentInfo));
            }
        }

        void Add(DateTime v)
        {
            Add(v, true);
        }

        void Add(DateTime dt, bool time)
        {
            short yr = (short)dt.Year;
            short mon = (byte)dt.Month, day = (byte)dt.Day;

            Add(yr);
            Write('-');
            if (mon < 10) Write('0');
            Add(mon);
            Write('-');
            if (day < 10) Write('0');
            Add(day);

            byte hr = (byte)dt.Hour, min = (byte)dt.Minute, sec = (byte)dt.Second;
            if (time)
            {
                Write(' '); // a space for separation
                if (hr < 10) Write('0');
                Add(hr);
                Write(':');
                if (min < 10) Write('0');
                Add(min);
                Write(':');
                if (sec < 10) Write('0');
                Add(sec);
            }
        }


        //
        // PUT
        //

        public void PutArr(Action a)
        {
            if (counts[level]++ > 0) Write(',');

            counts[++level] = 0;
            Write('{');

            if (a != null) a();

            Write('}');
            level--;
        }

        public void PutArr<T>(T[] v, ushort x = 0xffff) where T : IPersist
        {
            PutArr(delegate
            {
                for (int i = 0; i < v.Length; i++)
                {
                    PutObj(v[i], x);
                }
            });
        }

        public void PutObj(Action a)
        {
            if (counts[level]++ > 0) Write(',');

            counts[++level] = 0;
            Write('[');

            if (a != null) a();

            Write(']');
            level--;
        }

        public void PutObj<T>(T v, ushort x = 0xffff) where T : IPersist
        {
            PutObj(delegate
            {
                v.Save(this, x);
            });
        }


        //
        // SINK
        //

        public JTextBuild PutNull(string name)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add("null");

            return this;
        }

        public JTextBuild Put(string name, bool v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v ? "true" : "false");

            return this;
        }

        public JTextBuild Put(string name, short v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JTextBuild Put(string name, int v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JTextBuild Put(string name, long v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JTextBuild Put(string name, decimal v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JTextBuild Put(string name, Number v)
        {
            if (counts[level]++ > 0) Add(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v.integr);
            if (v.Pt)
            {
                Add('.');
                Add(v.fract);
            }
            return this;
        }

        public JTextBuild Put(string name, DateTime v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Add(v);

            return this;
        }

        public JTextBuild Put(string name, char[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Write('"');
            Add(v);
            Write('"');

            return this;
        }

        public JTextBuild Put(string name, string v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            Write('"');
            Add(v);
            Write('"');

            return this;
        }

        public JTextBuild Put(string name, byte[] v)
        {
            throw new NotImplementedException();
        }

        public JTextBuild Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public JTextBuild Put<T>(string name, T v, ushort x = 0xffff) where T : IPersist
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                PutObj(v, x);
            }

            return this;
        }

        public JTextBuild Put(string name, JObj v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                PutObj(delegate
                {
                    v.Save(this);
                });
            }

            return this;
        }

        public JTextBuild Put(string name, JArr v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                PutArr(delegate
                {
                    v.Save(this);
                });
            }

            return this;
        }

        public JTextBuild Put(string name, short[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');
                    Add(v[i]);
                }
                Write(']');
            }

            return this;
        }

        public JTextBuild Put(string name, int[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');
                    Add(v[i]);
                }
                Write(']');
            }

            return this;
        }

        public JTextBuild Put(string name, long[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');
                    Add(v[i]);
                }
                Write(']');
            }

            return this;
        }

        public JTextBuild Put(string name, string[] v)
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                Write('[');
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Write(',');
                    Write('"');
                    Add(v[i]);
                    Write('"');
                }
                Write(']');
            }

            return this;
        }

        public JTextBuild Put<T>(string name, T[] v, ushort x = 0xffff) where T : IPersist
        {
            if (counts[level]++ > 0) Write(',');

            if (name != null)
            {
                Write('"');
                Add(name);
                Write('"');
                Write(':');
            }

            if (v == null)
            {
                Add("null");
            }
            else
            {
                PutArr(v, x);
            }

            return this;
        }

        public override string ToString()
        {
            return new string(buffer, 0, count);
        }
    }
}