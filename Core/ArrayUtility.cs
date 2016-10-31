using System;

namespace Greatbone.Core
{
    public static class ArrayUtility
    {

        public static E[] Add<E>(this E[] arr, E v)
        {
            if (arr == null || arr.Length == 0)
            {
                return new E[] { v };
            }

            int len = arr.Length;
            E[] alloc = new E[len + 1];
            Array.Copy(arr, alloc, len);
            alloc[len] = v;
            return alloc;
        }

        public static E[] Add<E>(this E[] arr, params E[] v)
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

        public static E[] Remove<E>(this E[] arr, int index)
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

    }

}