using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// A text/plain model or comma-separate values. It can be used as UTF-8 string builder.
    ///
    public class Str : IDataInput
    {
        protected char[] charbuf;

        // number of chars
        protected int count;

        // combination of bytes
        int sum;

        // number of rest octets
        int rest;

        public Str(int capacity = 256)
        {
            charbuf = new char[capacity];
            sum = 0;
            rest = 0;
        }

        public int Count => count;

        public void Add(char c)
        {
            // ensure capacity
            int len = charbuf.Length; // old length
            if (count >= len)
            {
                int newlen = len * 4; // new length
                char[] buf = charbuf;
                charbuf = new char[newlen];
                Array.Copy(buf, 0, charbuf, 0, len);
            }
            charbuf[count++] = c;
        }

        // utf-8 decoding 
        public void Accept(int b)
        {
            // process a byte
            if (rest == 0)
            {
                if (b > 0xff) // if a char already
                {
                    Add((char) b);
                    return;
                }
                if (b < 0x80)
                {
                    Add((char) b); // single byte
                }
                else if (b >= 0xc0 && b < 0xe0)
                {
                    sum = (b & 0x1f) << 6;
                    rest = 1;
                }
                else if (b >= 0xe0 && b < 0xf0)
                {
                    sum = (b & 0x0f) << 12;
                    rest = 2;
                }
            }
            else if (rest == 1)
            {
                sum |= (b & 0x3f);
                rest--;
                Add((char) sum);
            }
            else if (rest == 2)
            {
                sum |= (b & 0x3f) << 6;
                rest--;
            }
        }

        public virtual void Clear()
        {
            count = 0;
            sum = 0;
            rest = 0;
        }

        public bool Get(string name, ref bool v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref double v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref decimal v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref DateTime v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref short[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref int[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref long[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref string[] v)
        {
            throw new NotImplementedException();
        }

        public bool Get(string name, ref Dictionary<string, string> v)
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D v, short proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool Get<D>(string name, ref D[] v, short proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D ToObject<D>(short proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(short proj = 0) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public void WriteData<R>(IDataOutput<R> snk) where R : IDataOutput<R>
        {
        }

        public IContent Dump()
        {
            var cont = new StrContent(true, true);
            cont.Add(charbuf, 0, count);
            return cont;
        }

        public bool DataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public bool Matches(string str)
        {
            if (str == null || str.Length != count) return false;

            for (int i = 0; i < count; i++)
            {
                if (charbuf[i] != str[i]) return false;
            }
            return true;
        }

        public override string ToString()
        {
            return new string(charbuf, 0, count);
        }
    }
}