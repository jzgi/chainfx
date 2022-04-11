using System;

namespace Chainly
{
    /// <summary>
    /// A field in a form model.
    /// </summary>
    public struct Field : IKeyable<string>
    {
        readonly string name;

        // can be string or string[]
        object value;

        // actual items in the value
        int items;

        internal readonly byte[] contentbuf;

        readonly int offset;

        readonly int count;

        internal Field(string key, string v)
        {
            this.name = key;
            value = v;
            items = 1;
            contentbuf = null;
            offset = 0;
            count = 0;
        }

        internal Field(string key, string filename, byte[] contentbuf, int offset, int count)
        {
            this.name = key;
            value = filename;
            items = 1;
            this.contentbuf = contentbuf;
            this.offset = offset;
            this.count = count;
        }

        public string Key => name;

        internal void Add(string v)
        {
            if (items == 1) // a single string
            {
                var arr = new string[8];
                arr[0] = (string) value;
                arr[1] = v;
                value = arr;
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

        string First => (items == 0) ? null :
            (items == 1) ? (string) value : ((string[]) value)[0];

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

        public static implicit operator char(Field v)
        {
            string str = v.First;
            if (!string.IsNullOrEmpty(str))
            {
                return str[0];
            }
            return '\0';
        }

        public static implicit operator short(Field v)
        {
            string str = v.First;
            if (str != null)
            {
                if (short.TryParse(str, out var n))
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
                if (int.TryParse(str, out var n))
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
                if (long.TryParse(str, out var n))
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
                if (double.TryParse(str, out var n))
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
                if (decimal.TryParse(str, out var n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator DateTime(Field v)
        {
            string str = v.First;
            if (TextUtility.TryParseDate(str, out var dt))
            {
                return dt;
            }
            return default;
        }

        public static implicit operator string(Field v)
        {
            return v.First;
        }

        public static implicit operator ValueTuple<int, string>(Field v)
        {
            string str = v.First;
            if (string.IsNullOrEmpty(str))
            {
                return (0, null);
            }
            int pos = 0;
            var a = str.ParseInt(ref pos);
            var b = str.ParseString(ref pos);
            return (a, b);
        }

        public static implicit operator byte[](Field v)
        {
            byte[] buf = v.contentbuf;
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
            if (v.count == 0)
            {
                return default;
            }
            return new ArraySegment<byte>(v.contentbuf, v.offset, v.count);
        }

        public static implicit operator short[](Field v)
        {
            int len = v.items;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string) v.value;
                return new[] {short.TryParse(str, out var n) ? n : (short) 0};
            }

            var strs = (string[]) v.value;
            var arr = new short[len];
            for (int i = 0; i < len; i++)
            {
                arr[i] = short.TryParse(strs[i], out var n) ? n : (short) 0;
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
                return new[] {int.TryParse(str, out var n) ? n : 0};
            }

            var strs = (string[]) v.value;
            var arr = new int[len];
            for (int i = 0; i < len; i++)
            {
                arr[i] = int.TryParse(strs[i], out var n) ? n : 0;
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
                return new[] {long.TryParse(str, out var n) ? n : 0};
            }
            var strs = (string[]) v.value;
            var arr = new long[len];
            for (int i = 0; i < len; i++)
            {
                arr[i] = long.TryParse(strs[i], out var n) ? n : 0;
            }
            return arr;
        }

        public static implicit operator DateTime[](Field v)
        {
            int len = v.items;
            if (len == 0) return null;
            if (len == 1)
            {
                string str = (string) v.value;
                DateTime.TryParse(str, out var dt);
                return new[] {dt};
            }
            var strs = (string[]) v.value;
            var arr = new DateTime[len];
            for (int i = 0; i < len; i++)
            {
                DateTime.TryParse(strs[i], out var dt);
                arr[i] = dt;
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

            var strs = (string[]) v.value;
            var arr = new string[len];
            for (int i = 0; i < len; i++)
            {
                arr[i] = strs[i];
            }
            return arr;
        }
    }
}