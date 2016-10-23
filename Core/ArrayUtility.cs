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
            E[] all = new E[len + 1];
            Array.Copy(arr, all, len);
            all[len] = v;
            return all;
        }

        public static E[] Add<E>(this E[] arr, params E[] v)
        {
            if (arr == null || arr.Length == 0)
            {
                return v;
            }

            int len = arr.Length;
            int vlen = v.Length;
            E[] all = new E[len + vlen];
            Array.Copy(arr, all, len);
            Array.Copy(v, 0, all, len, vlen);
            return all;
        }

    }

}