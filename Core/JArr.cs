using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An array data model.
    /// </summary>
    public class JArr
    {
        const int InitialCapacity = 16;

        JMember[] elements;

        int count;

        internal JArr(int capacity = InitialCapacity)
        {
            elements = new JMember[capacity];
            count = 0;
        }

        public JMember this[int index]
        {
            get { return elements[index]; }
        }

        public int Count => count;

        internal void Add(JMember elem)
        {
            int len = elements.Length;
            if (count >= len)
            {
                JMember[] @new = new JMember[len * 4];
                Array.Copy(elements, @new, len);
                elements = @new;
            }
            elements[count++] = elem;
        }

    }
}