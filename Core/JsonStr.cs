using System;

namespace Greatbone.Core
{
    internal struct JsonStr
    {
        char[] chars;

        int count;

        internal JsonStr(int initial)
        {
            chars = new char[initial];
            count = 0;
        }
        internal void Add(byte c)
        {
            int olen = chars.Length;
            if (count == olen)
            {
                char[] @new = new char[olen * 4];
                Array.Copy(chars, 0, @new, 0, olen);
                chars = @new;
            }
            chars[count++] = (char)c;
        }

        public override string ToString()
        {
            return new string(chars, 0, count);
        }

    }
}