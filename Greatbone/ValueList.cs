using System;

namespace Greatbone
{
    /// <summary>
    /// A lightweight alternative to the List class. The internal array is created on demand.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ValueList<T>
    {
        const int DefaultCap = 16;

        readonly int capacity;

        T[] array;

        int count;

        public ValueList(int capacity = DefaultCap)
        {
            this.capacity = capacity;
            array = null;
            count = 0;
        }

        public int Count => count;

        public T this[int idx] => array[idx];

        public void Add(T v)
        {
            // ensure capacity
            if (array == null)
            {
                array = new T[capacity <= 0 ? DefaultCap : capacity];
            }
            else
            {
                int len = array.Length;
                if (count >= len)
                {
                    T[] alloc = new T[len * 4];
                    Array.Copy(array, 0, alloc, 0, len);
                    array = alloc;
                }
            }
            array[count++] = v;
        }

        public T[] ToArray()
        {
            if (count > 0)
            {
                T[] alloc = new T[count];
                Array.Copy(array, 0, alloc, 0, count);
                return alloc;
            }
            return null;
        }
    }
}