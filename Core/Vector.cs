using System;

namespace Greatbone.Core
{
    public struct Vector<T>
    {
        T[] array;

        int count;

        public Vector(int capacity)
        {
            array = new T[capacity];
            count = 0;
        }

        public void Add(T v)
        {
            // ensure capacity
            int len = array.Length;
            if (count >= len)
            {
                T[] alloc = new T[len * 4];
                Array.Copy(array, 0, alloc, 0, len);
                array = alloc;
            }
            array[count++] = v;
        }

        public T[] ToArray()
        {
            T[] alloc = new T[count];
            Array.Copy(array, 0, alloc, 0, count);
            return alloc;
        }
    }
}