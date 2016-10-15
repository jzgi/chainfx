using System;

namespace Greatbone.Core
{

    /// <summary>
    /// A character string builder that supports UTF-8 decoding.
    /// </summary>
    class Str
    {
        const int InitialCapacity = 256;

        char[] buffer;

        int count;

        int sum; // combination of bytes

        int rest; // number of rest octets

        internal Str(int capacity = InitialCapacity)
        {
            buffer = new char[capacity];
            count = 0;
            sum = 0;
            rest = 0;
        }

        internal void Add(char c)
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

        internal void Add(byte b)
        {
            if (rest == 0)
            {
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

        public void Clear()
        {
            count = 0;
            sum = 0;
            rest = 0;
        }

        public override string ToString()
        {
            return new string(buffer, 0, count);
        }
    }
}