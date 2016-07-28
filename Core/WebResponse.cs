using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    ///
    /// The wrapper of a HTTP response, providing efficient output methods and cache control.
    ///
    public class WebResponse : IDataOutput
    {
        // the underlying implementation of a response
        private readonly HttpResponse _impl;

        // hexidecimal characters
        private static readonly char[] Hex =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        // possible chars for representing a number as a string
        private static readonly byte[] Digits =
        {
            (byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7', (byte) '8', (byte) '9'
        };

        private const byte Minus = (byte)'-';

        private static readonly byte[] Bytes = { 1, 10, 100 };

        private static readonly short[] Shorts = { 1, 10, 100, 1000, 10000 };

        private static readonly int[] Ints =
        {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 1000000000
        };

        private static readonly long[] Longs =
        {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 1000000000, 100000000000L, 1000000000000L, 10000000000000L, 100000000000000L, 100000000000000L, 10000000000000000L, 100000000000000000L, 1000000000000000000L
        };

        //
        // output buffer

        private byte[] _buffer; // NOTE: HttpResponseStream doesn't have internal buffer

        // the offset in buffer from where the content starts
        private int _offset;

        // number of bytes
        private int _count;

        // byte-wise etag checksum, for text-based output only
        private ulong _checksum;


        internal WebResponse(HttpResponse impl)
        {
            _impl = impl;
        }

        private void AddByte(byte b)
        {
            // double the capacity as needed
            int len = _buffer.Length;
            if (_count == len)
            {
                byte[] old = _buffer;
                _buffer = new byte[len * 2];
                Array.Copy(old, _buffer, len);
            }
            _buffer[_count++] = b; // append to the buffer

            // calculate checksum
            ulong cs = _checksum;
            cs ^= b; // XOR
            _checksum = cs >> 57 | cs << 7; // circular left shift 7 bit
        }

        public void Put(bool v)
        {
            AddByte((byte)'1');
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
            for (int i = Bytes.Length - 1; i >= 0; i--)
            {
                int bas = Bytes[i];
                int q = x / bas;
                x %= bas;
                if (q != 0 || bgn)
                {
                    AddByte(Digits[q]);
                    bgn = true;
                }
            }
            AddByte(Digits[v]); // last reminder
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

        public void Put(int v)
        {
            if (v == 0)
            {
                Put('0');
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

        public void Put(long v)
        {
            if (v == 0)
            {
                Put('0');
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

        public void Put(decimal v)
        {
            Put(v, true);
        }

        // sign mask
        private const int Sign = unchecked((int)0x80000000);

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
            short yr = (short)dt.Year;
            byte mon = (byte)dt.Month, day = (byte)dt.Day;

            Put(yr);
            Put('-');
            if (mon < 10) Put('0');
            AddByte(mon);
            Put('-');
            if (day < 10) Put('0');
            AddByte(day);

            byte hr = (byte)dt.Hour, min = (byte)dt.Minute, sec = (byte)dt.Second;
            if (time)
            {
                Put(' '); // a space for separation
                if (hr < 10) Put('0');
                AddByte(hr);
                Put(':');
                if (min < 10) Put('0');
                AddByte(min);
                Put(':');
                if (sec < 10) Put('0');
                AddByte(sec);
            }
        }

        public void Put(char c)
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

        public void SetContent(string ctype, byte[] buffer, int offset, int count)
        {
            if (ctype != null)
            {
                _impl.ContentType = ctype;
            }
            _buffer = buffer;
            _offset = offset;
            _count = count;
            _impl.ContentLength = count;
        }

        public void Put(string name, int value)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(' ');
            Put(':');
            Put(' ');
            Put(value);
        }

        public void Put(string name, decimal value)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(' ');
            Put(':');
            Put(' ');
            Put(value);
        }

        public void Put(string name, string value)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(' ');
            Put(':');
            Put(' ');
            Put(value);
        }

        public void Put<T>(string name, List<T> value) where T : IData
        {
            foreach (T v in value)
            {
                v.To(this, -1);
            }
        }

        public void PutStart()
        {
            throw new System.NotImplementedException();
        }

        public void PutEnd()
        {
            throw new System.NotImplementedException();
        }


        public int StatusCode
        {
            get { return _impl.StatusCode; }
            set { _impl.StatusCode = value; }
        }

        public void Redirect(string location)
        {
            _impl.Redirect(location);
        }

        public void Redirect(string location, bool permanent)
        {
            _impl.Redirect(location, permanent);
        }

        internal Task SendAsyncTask()
        {
            if (_count > 0)
            {
                _impl.ContentLength = _count;
                return _impl.Body.WriteAsync(_buffer, _offset, _count);
            }
            return null;
        }
    }
}