using System;

namespace Greatbone
{
    /// <summary>
    /// A set of frequently-used array operations.
    /// </summary>
    public static class ArrayUtility
    {
        public static E[] AddOf<E>(this E[] arr, E v, int limit = 0)
        {
            if (arr == null || arr.Length == 0)
            {
                return new[] {v};
            }
            int len = arr.Length;
            E[] alloc = new E[len + 1];
            Array.Copy(arr, alloc, len);
            alloc[len] = v;
            return alloc;
        }

        public static E[] AddOf<E>(this E[] arr, params E[] v)
        {
            if (arr == null || arr.Length == 0)
            {
                return v;
            }
            int len = arr.Length;
            int vlen = v.Length;
            E[] alloc = new E[len + vlen];
            Array.Copy(arr, alloc, len);
            Array.Copy(v, 0, alloc, len, vlen);
            return alloc;
        }

        public static E[] RemovedOf<E>(this E[] arr, int index)
        {
            if (arr == null) return null;

            int len = arr.Length;

            if (index >= len || index < 0) return arr;

            E[] alloc = new E[len - 1];
            Array.Copy(arr, 0, alloc, 0, index);
            int next = index + 1;
            Array.Copy(arr, next, alloc, index, len - next);
            return alloc;
        }

        public static E[] RemovedOf<E>(this E[] arr, Predicate<E> cond)
        {
            if (arr == null) return null;

            int len = arr.Length;

            if (len == 1 && cond(arr[0])) return null;

            for (int i = 0; i < len; i++)
            {
                E e = arr[i];
                if (cond(e))
                {
                    E[] alloc = new E[len - 1];
                    Array.Copy(arr, 0, alloc, 0, i);
                    int next = i + 1;
                    Array.Copy(arr, next, alloc, i, len - next);
                    return alloc;
                }
            }
            return arr;
        }

        public static E First<E>(this E[] arr, Predicate<E> cond)
        {
            if (arr != null)
            {
                int len = arr.Length;
                for (int i = 0; i < len; i++)
                {
                    E e = arr[i];
                    if (cond(e)) return e;
                }
            }
            return default;
        }

        public static int IndexOf<E>(this E[] arr, Predicate<E> cond)
        {
            if (arr != null)
            {
                int len = arr.Length;
                for (int i = 0; i < len; i++)
                {
                    E e = arr[i];
                    if (cond(e)) return i;
                }
            }
            return -1;
        }

        public static bool IsNullOrEmpty<E>(this E[] arr)
        {
            return arr == null || arr.Length == 0;
        }

        public static bool Contains<V>(this V[] arr, V v)
        {
            if (v != null && arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].Equals(v)) return true;
                }
            }
            return false;
        }


        public static bool SameAs<E>(this E[] arr, E[] another)
        {
            if (arr == null && another == null)
            {
                return true;
            }
            if (arr != null && another != null && arr.Length == another.Length)
            {
                int len = arr.Length;
                for (int i = 0; i < len; i++)
                {
                    if (!arr[i].Equals(another[i])) return false;
                }
                return true;
            }
            return false;
        }
    }
}