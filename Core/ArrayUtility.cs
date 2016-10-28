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
            E[] @new = new E[len + 1];
            Array.Copy(arr, @new, len);
            @new[len] = v;
            return @new;
        }

        public static E[] Add<E>(this E[] arr, params E[] v)
        {
            if (arr == null || arr.Length == 0)
            {
                return v;
            }

            int len = arr.Length;
            int vlen = v.Length;
            E[] @new = new E[len + vlen];
            Array.Copy(arr, @new, len);
            Array.Copy(v, 0, @new, len, vlen);
            return @new;
        }

        public static E[] Remove<E>(this E[] arr, int index)
        {
            if (arr == null) return null;

            int len = arr.Length;

            if (index >= len || index < 0) return arr;

            E[] @new = new E[len - 1];
            Array.Copy(arr, 0, @new, 0, index);
            int next = index + 1;
            Array.Copy(arr, next, @new, index, len - next);
            return @new;
        }

    }

}