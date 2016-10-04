using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An array data model.
    /// </summary>
    public class Arr
    {
        const int InitialCapacity = 16;

        Member[] elements;

        int count;

        internal Arr(int capacity = InitialCapacity)
        {
            elements = new Member[capacity];
            count = 0;
        }

        public Member this[int index]
        {
            get { return elements[index]; }
        }

        public int Count => count;

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