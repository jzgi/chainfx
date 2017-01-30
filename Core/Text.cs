using System;

namespace Greatbone.Core
{
    ///
    /// A reusable string builder that supports UTF-8 decoding.
    ///
    public class Text : IModel
    {
        protected char[] buf;

        // number of chars
        protected int count;

        // combination of bytes
        int sum;

        // number of rest octets
        int rest;

        bool one;

        public Text(int capacity = 256)
        {
            buf = new char[capacity];
            sum = 0;
            rest = 0;
        }

        public int Count => count;

        public bool Single => one;

        public void Add(char c)
        {
            // ensure capacity
            int len = buf.Length; // old length
            if (count >= len)
            {
                int newlen = len * 4; // new length
                char[] buf = this.buf;
                this.buf = new char[newlen];
                Array.Copy(buf, 0, this.buf, 0, len);
            }
            buf[count++] = c;
        }

        // utf-8 decoding 
        public void Accept(int b)
        {
            // process a byte
            if (rest == 0)
            {
                if (b > 0xff) // if a char already
                {
                    Add((char)b);
                    return;
                }
                if (b < 0x80)
                {
                    Add((char)b); // single byte
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
                Add((char)sum);
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

        public override string ToString()
        {
            return new string(buf, 0, count);
        }

        public void Dump<R>(ISink<R> snk) where R : ISink<R>
        {
        }

        public C Dump<C>() where C : IContent, ISink<C>, new()
        {
            C cont = new C();
            Dump(cont);
            return cont;
        }

    }
}