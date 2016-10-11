using System;

namespace Greatbone.Core
{
    public static class ArrayUtility
    {

        public static T[] Concat<T>(this T[] arr, T v)
        {
            int len = arr.Length;
            T[] all = new T[len + 1];
            Array.Copy(arr, all, len);
            all[len] = v;
            return all;
        }

        public static T[] Concat<T>(this T[] arr, params T[] v)
        {
            int len = arr.Length;
            int vlen = v.Length;
            T[] all = new T[len + vlen];
            Array.Copy(arr, all, len);
            Array.Copy(v, 0, all, len, vlen);
            return all;
        }

    }

}