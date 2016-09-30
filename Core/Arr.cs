using System;

namespace Greatbone.Core
{
    public class Arr
    {
        Member[] elements;

        int count;

        internal Arr(int capacity)
        {
            elements = new Member[capacity];
            count = 0;
        }

        public Member this[int index]
        {
            get { return elements[index]; }
        }

        internal void Add(Member elem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                Member[] @new = new Member[len * 4];
                Array.Copy(elements, @new, len);
                elements = @new;
            }
            elements[count++] = elem;
        }

    }
}