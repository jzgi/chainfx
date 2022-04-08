using System;
using System.Collections.Generic;

namespace FabricQ
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
            var alloc = new E[len + vlen];
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
            var lst = new ValueList<E>();
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
                var alloc = new E[len + count];
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

        public static E[] RemovedOf<E>(this E[] arr, E v)
        {
            if (arr == null) return null;

            int idx = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(v))
                {
                    idx = i;
                    break;
                }
            }
            if (idx == -1) return arr;

            int nlen = arr.Length - 1;

            if (nlen == 0) return null;
            var alloc = new E[nlen];
            if (idx > 0)
            {
                Array.Copy(arr, 0, alloc, 0, idx);
            }
            if (idx < nlen)
            {
                Array.Copy(arr, idx + 1, alloc, idx, nlen - idx);
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
                var e = arr[i];
                if (cond(e))
                {
                    var alloc = new E[len - 1];
                    Array.Copy(arr, 0, alloc, 0, i);
                    int next = i + 1;
                    Array.Copy(arr, next, alloc, i, len - next);
                    return alloc;
                }
            }

            return arr;
        }

        public static E[] RemovedOf<E>(this E[] arr, E[] cond)
        {
            var lst = new List<E>(arr);
            lst.RemoveAll(e => cond.IndexOf(e) != -1);
            return lst.ToArray();
        }

        public static E[] ReplaceOf<E>(this E[] arr, E old, E @new)
        {
            if (arr == null) return null;

            int idx = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(old))
                {
                    idx = i;
                    break;
                }
            }
            if (idx == -1) return arr;

            arr[idx] = @new;

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