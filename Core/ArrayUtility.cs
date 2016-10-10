using System;

namespace Greatbone.Core
{
    public static class ArrayUtility
    {

        public static T[] Concat<T>(this T[] arr, T v)
        {
            int len = arr.Length;
            T[] @new = new T[len + 1];
            Array.Copy(arr, @new, len);
            @new[len] = v;
            return @new;
        }

        public static T[] Concat<T>(this T[] arr, params T[] v)
        {
            int len = arr.Length;
            int vlen = v.Length;
            T[] @new = new T[len + vlen];
            Array.Copy(arr, @new, len);
            Array.Copy(v, 0, @new, len, vlen);
            return @new;
        }

    }

}