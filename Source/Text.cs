using System;

namespace ChainFx
{
    /// <summary>
    /// A text/plain model or comma-separate (text/csv) lines and values (CSV). Also it can be used as a UTF-8 string builder.
    /// </summary>
    public class Text : ISource
    {
        protected char[] charbuf;

        // number of chars
        protected int count;

        // combination of bytes
        int sum;

        // number of rest octets
        int rest;

        public Text(int capacity = 512)
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

        /// Utf-8 decoding 
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
            return false;
        }

        public bool Get(string name, ref char v)
        {
            return false;
        }

        public bool Get(string name, ref short v)
        {
            return false;
        }

        public bool Get(string name, ref int v)
        {
            return false;
        }

        public bool Get(string name, ref long v)
        {
            return false;
        }

        public bool Get(string name, ref double v)
        {
            return false;
        }

        public bool Get(string name, ref decimal v)
        {
            return false;
        }

        public bool Get(string name, ref DateTime v)
        {
            return false;
        }

        public bool Get(string name, ref Guid v)
        {
            return false;
        }

        public bool Get(string name, ref string v)
        {
            return false;
        }

        public bool Get(string name, ref ArraySegment<byte> v)
        {
            return false;
        }

        public bool Get(string name, ref byte[] v)
        {
            return false;
        }

        public bool Get(string name, ref short[] v)
        {
            return false;
        }

        public bool Get(string name, ref int[] v)
        {
            return false;
        }

        public bool Get(string name, ref long[] v)
        {
            return false;
        }

        public bool Get(string name, ref string[] v)
        {
            return false;
        }

        public bool Get(string name, ref JObj v)
        {
            return false;
        }

        public bool Get(string name, ref JArr v)
        {
            return false;
        }

        public bool Get(string name, ref XElem v)
        {
            return false;
        }

        public bool Get<D>(string name, ref D v, short msk = 0xff) where D : IData, new()
        {
            return false;
        }

        public bool Get<D>(string name, ref D[] v, short msk = 0xff) where D : IData, new()
        {
            return false;
        }

        //
        // ENTITY
        //

        public D ToObject<D>(short msk = 0xff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public D[] ToArray<D>(short msk = 0xff) where D : IData, new()
        {
            throw new NotImplementedException();
        }

        public bool IsDataSet => false;

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public void Write<C>(C cnt) where C : ContentBuilder, ISink
        {
            throw new NotImplementedException();
        }

        public IContent Dump()
        {
            var cnt = new TextBuilder(true, 4096);
            cnt.Add(charbuf, 0, count);
            return cnt;
        }

        public bool Equals(string str)
        {
            if (str == null || str.Length != count) return false;

            for (int i = 0; i < count; i++)
            {
                if (charbuf[i] != str[i]) return false;
            }
            return true;
        }

        public bool StartsWith(string str)
        {
            if (str == null || str.Length > count) return false;

            for (int i = 0; i < count; i++)
            {
                if (charbuf[i] != str[i]) return false;
            }
            return true;
        }

        public bool EndsWith(string str)
        {
            if (str == null || str.Length > count) return false;

            for (int i = count - 1; i >= 0; i--)
            {
                if (charbuf[i] != str[i]) return false;
            }
            return true;
        }

        public string ToString(int start, int len)
        {
            return new string(charbuf, start, len);
        }

        public override string ToString()
        {
            return new string(charbuf, 0, count);
        }
    }
}