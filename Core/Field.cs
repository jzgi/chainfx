using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public struct Field : IRollable
    {
        readonly string name;

        // can be string or string[]
        object value;

        // actual items in the value
        int items;

        // data buffer of a part, if any
        internal readonly byte[] databuf;

        int count;

        internal Field(string name, string v)
        {
            this.name = name;
            value = v;
            items = 1;
            databuf = null;
            count = 0;
            Err = null;
        }

        internal Field(string name, string filename, byte[] databuf, int count)
        {
            this.name = name;
            value = filename;
            items = 1;
            this.databuf = databuf;
            this.count = count;
            Err = null;
        }

        public string Name => name;

        public string Err { get; set; }

        internal void Add(string v)
        {
            if (items == 1) // a single string
            {
                string old = (string) value;
                string[] arr = new string[8];
                arr[0] = old;
                arr[1] = v;
                items = 2;
            }
            else
            {
                // ensure capacity
                string[] arr = (string[]) value;
                int len = arr.Length;
                if (items >= len)
                {
                    string[] alloc = new string[len * 4];
                    Array.Copy(arr, 0, alloc, 0, len);
                    value = arr = alloc;
                }
                arr[items++] = v;
            }
        }

        string First => (items == 0) ? null : (items == 1) ? (string) value : ((string[]) value)[0];

        //
        // CONVERSION
        //

        public static implicit operator bool(Field v)
        {
            string str = v.First;
            if (str != null)
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(Field v)
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

        public static implicit operator int(Field v)
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

        public static implicit operator long(Field v)
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

        public static implicit operator double(Field v)
        {
            string str = v.First;
            if (str != null)
            {
                double n;
                if (double.TryParse(str, out n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator decimal(Field v)
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

        public static implicit operator DateTime(Field v)
        {
            return default(DateTime);
        }

        public static implicit operator NpgsqlPoint(Field v)
        {
            return default(NpgsqlPoint);
        }

        public static implicit operator char[](Field v)
        {
            string str = v.First;
            return str?.ToCharArray();
        }

        public static implicit operator string(Field v)
        {
            return v.First;
        }

        public static implicit operator byte[](Field v)
        {
            byte[] buf = v.databuf;
            if (buf != null)
            {
                if (v.count == buf.Length)
                {
                    return buf;
                }
            }
            return null;
        }

        public static implicit operator ArraySegment<byte>(Field v)
        {
            return new ArraySegment<byte>(v.databuf, 0, v.count);
        }

        public static implicit operator short[](Field v)
        {
            int len = v.items;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string) v.value;
                short n;
                return new[] {short.TryParse(str, out n) ? n : (short) 0};
            }

            string[] strs = (string[]) v.value;
            short[] arr = new short[len];
            for (int i = 0; i < len; i++)
            {
                short n;
                arr[i] = short.TryParse(strs[i], out n) ? n : (short) 0;
            }
            return arr;
        }

        public static implicit operator int[](Field v)
        {
            int len = v.items;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string) v.value;
                int n;
                return new[] {int.TryParse(str, out n) ? n : 0};
            }

            string[] strs = (string[]) v.value;
            int[] arr = new int[len];
            for (int i = 0; i < len; i++)
            {
                int n;
                arr[i] = int.TryParse(strs[i], out n) ? n : 0;
            }
            return arr;
        }

        public static implicit operator long[](Field v)
        {
            int len = v.items;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string) v.value;
                long n;
                return new[] {long.TryParse(str, out n) ? n : 0};
            }
            string[] strs = (string[]) v.value;
            long[] arr = new long[len];
            for (int i = 0; i < len; i++)
            {
                long n;
                arr[i] = long.TryParse(strs[i], out n) ? n : 0;
            }
            return arr;
        }

        public static implicit operator string[](Field v)
        {
            int len = v.items;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string) v.value;
                return new[] {str};
            }

            string[] strs = (string[]) v.value;
            string[] arr = new string[len];
            for (int i = 0; i < len; i++)
            {
                arr[i] = strs[i];
            }
            return arr;
        }
    }
}