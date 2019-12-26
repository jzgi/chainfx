using System;

namespace Greatbone
{
    /// <summary>
    /// A set of commonly-used array operations.
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
            E[] alloc;
            if (limit > 0 && limit <= len)
            {
                alloc = new E[limit];
                Array.Copy(arr, len - limit + 1, alloc, 0, limit - 1);
                alloc[limit - 1] = v;
            }
            else
            {
                alloc = new E[len + 1];
                Array.Copy(arr, alloc, len);
                alloc[len] = v;
            }

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

        public static E[] MergeOf<E>(this E[] arr, params E[] v) where E : IEquatable<E>
        {
            if (arr == null || arr.Length == 0)
            {
                return v;
            }

            int len = arr.Length;
            int vlen = v.Length;

            ValueList<E> lst = new ValueList<E>();
            for (int i = 0; i < vlen; i++) // out loop
            {
                var t = v[i];
                if (t == null) continue;

                bool dup = false; // found duplicate
                for (int k = 0; k < len; k++) // match among arr elements
                {
                    var a = arr[k];
                    if (a == null) continue;

                    if (a.Equals(t))
                    {
                        dup = true;
                        break;
                    }
                }

                if (!dup)
                {
                    lst.Add(t);
                }
            }

            int count = lst.Count;
            if (count > 0)
            {
                E[] alloc = new E[len + count];
                Array.Copy(arr, alloc, len);
                // copy new elements
                for (int i = 0; i < count; i++)
                {
                    alloc[len + i] = lst[i];
                }

                return alloc;
            }

            return arr;
        }

        public static E[] RemovedOf<E>(this E[] arr, int index)
        {
            if (arr == null) return null;

            int nlen = arr.Length - 1;

            if (nlen == 0) return null;

            if (index > nlen || index < 0) return arr;

            E[] alloc = new E[nlen];
            if (index > 0)
            {
                Array.Copy(arr, 0, alloc, 0, index);
            }
            if (index < nlen)
            {
                Array.Copy(arr, index + 1, alloc, index, nlen - index);
            }
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

        public static E Last<E>(this E[] arr, Predicate<E> cond)
        {
            if (arr != null)
            {
                int len = arr.Length;
                for (int i = len - 1; i > 0; i--)
                {
                    E e = arr[i];
                    if (cond(e)) return e;
                }
            }

            return default;
        }

        public static V[] Exract<E, V>(this E[] arr, Func<E, V> expr)
        {
            var lst = new ValueList<V>();
            if (arr != null)
            {
                int len = arr.Length;
                for (int i = 0; i < len; i++)
                {
                    E e = arr[i];
                    lst.Add(expr(e));
                }
            }
            return lst.ToArray();
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

        public static int IndexOf<V>(this V[] arr, V v)
        {
            if (v != null && arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].Equals(v)) return i;
                }
            }
            return -1;
        }


        public static bool IsSameAs<E>(this E[] arr, E[] another)
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