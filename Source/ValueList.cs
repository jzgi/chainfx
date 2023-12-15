using System;
using System.Collections;
using System.Collections.Generic;

namespace ChainFX
{
    /// <summary>
    /// A lightweight alternative to the List class. The internal array is created on demand.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ValueList<T> : IList<T>
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

        public bool IsReadOnly => true;

        public int Count => count;

        public T this[int idx]
        {
            get => array[idx];
            set { }
        }

        public short Tag { get; set; }

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
                    T[] alloc = new T[len * 2];
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
                var alloc = new T[count];
                Array.Copy(array, 0, alloc, 0, count);
                return alloc;
            }

            return null;
        }

        public void Clear()
        {
            count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) > -1;
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            if (item != null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (item.Equals(array[i]))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
        }

        public void RemoveAt(int index)
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }
}