using System;

namespace Greatbone.Core
{

    /// <summary>
    /// A string builder that supports UTF-8 decoding.
    /// </summary>
    internal class Str
    {
        const int InitialCapacity = 256;

        char[] buffer;

        int count;

        // started escape sequence
        bool esc;

        // UTF-8 sequence
        byte b1, b2, b3;

        int octets; // number of octets

        int ordinal; // current

        internal Str(int capacity = InitialCapacity)
        {
            buffer = new char[capacity];
            count = 0;
            esc = false;
            b1 = b2 = b3 = 0;
            octets = 0;
            ordinal = 0;
        }

        internal void Add(char c)
        {
            // ensure capacity
            int olen = buffer.Length;
            if (count == olen)
            {
                char[] @new = new char[olen * 4];
                Array.Copy(buffer, 0, @new, 0, olen);
                buffer = @new;
            }

        
            buffer[count++] = c;
        }

        internal void Add(byte b)
        {
            char c;
            // UTF-8
            if (b < 0x80)
            {
                c = (char)b;
            }
            else if (b < 10)
            {
            }
            if (octets == 0)
            {
                if ((b & 0xc0) == 0xc0)
                {
                }
            }
        }

        public void Clear()
        {
            count = 0;
        }

        public override string ToString()
        {
            return new string(buffer, 0, count);
        }
    }
}