using System;

namespace Greatbone.Core
{
    public class Arr
    {
        Elem[] elements;

        int count;

        internal Arr(int capacity)
        {
            elements = new Elem[capacity];
            count = 0;
        }

        public Elem this[int index]
        {
            get { return elements[index]; }
        }

        internal void Add(Elem elem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                Elem[] @new = new Elem[len * 4];
                Array.Copy(elements, @new, len);
                elements = @new;
            }
            elements[count++] = elem;
        }

    }
}