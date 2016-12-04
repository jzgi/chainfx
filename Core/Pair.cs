using System;

namespace Greatbone.Core
{
    public struct Pair : IRollable
    {
        readonly string name;

        // string or string array
        object values;

        int count;

        public string Name => name;

        internal Pair(string name, string v)
        {
            this.name = name;
            values = v;
            count = 1;
        }

        internal void Add(string v)
        {
            if (count == 1) // a single string
            {
                string old = (string)values;
                string[] arr = new string[8];
                arr[0] = old;
                arr[1] = v;
                count = 2;
            }
            else
            {
                // ensure capacity
                string[] arr = (string[])values;
                int len = arr.Length;
                if (count >= len)
                {
                    string[] alloc = new string[len * 4];
                    Array.Copy(arr, 0, alloc, 0, len);
                    values = arr = alloc;
                }
                arr[count++] = v;
            }
        }

        string First => (count == 0) ? null : (count == 1) ? (string)values : ((string[])values)[0];

        //
        // CONVERSION
        //

        public static implicit operator bool(Pair v)
        {
            string str = v.First;
            if (str != null)
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(Pair v)
        {
            string str = v.First;
            if (str != null)
            {
                short n;
                if (short.TryParse(str, out n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator int(Pair v)
        {
            string str = v.First;
            if (str != null)
            {
                int n;
                if (int.TryParse(str, out n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator long(Pair v)
        {
            string str = v.First;
            if (str != null)
            {
                long n;
                if (long.TryParse(str, out n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator decimal(Pair v)
        {
            string str = v.First;
            if (str != null)
            {
                decimal n;
                if (decimal.TryParse(str, out n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator Number(Pair v)
        {
            return default(Number);
        }

        public static implicit operator DateTime(Pair v)
        {
            return default(DateTime);
        }

        public static implicit operator char[] (Pair v)
        {
            string str = v.First;
            return str?.ToCharArray();
        }

        public static implicit operator string(Pair v)
        {
            return v.First;
        }

        public static implicit operator byte[] (Pair v)
        {
            return null;
        }

        public static implicit operator short[] (Pair v)
        {
            int len = v.count;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string)v.values;
                short n;
                return new short[] { short.TryParse(str, out n) ? n : (short)0 };
            }
            else
            {
                string[] strs = (string[])v.values;
                short[] arr = new short[len];
                for (int i = 0; i < len; i++)
                {
                    short n;
                    arr[i] = short.TryParse(strs[i], out n) ? n : (short)0;
                }
                return arr;
            }
        }

        public static implicit operator int[] (Pair v)
        {
            int len = v.count;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string)v.values;
                int n;
                return new int[] { int.TryParse(str, out n) ? n : 0 };
            }
            else
            {
                string[] strs = (string[])v.values;
                int[] arr = new int[len];
                for (int i = 0; i < len; i++)
                {
                    int n;
                    arr[i] = int.TryParse(strs[i], out n) ? n : 0;
                }
                return arr;
            }
        }

        public static implicit operator long[] (Pair v)
        {
            int len = v.count;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string)v.values;
                long n;
                return new long[] { long.TryParse(str, out n) ? n : 0 };
            }
            else
            {
                string[] strs = (string[])v.values;
                long[] arr = new long[len];
                for (int i = 0; i < len; i++)
                {
                    long n;
                    arr[i] = long.TryParse(strs[i], out n) ? n : 0;
                }
                return arr;
            }
        }

        public static implicit operator string[] (Pair v)
        {
            int len = v.count;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string)v.values;
                return new string[] { str };
            }
            else
            {
                string[] strs = (string[])v.values;
                string[] arr = new string[len];
                for (int i = 0; i < len; i++)
                {
                    arr[i] = strs[i];
                }
                return arr;
            }
        }
    }
}