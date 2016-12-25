using System;
using NpgsqlTypes;

namespace Greatbone.Core
{
    public struct Attr : IRollable
    {
        readonly string name;

        string value;

        public string Name => name;

        internal Attr(string key, string v)
        {
            this.name = key;
            value = v;
        }

        public static implicit operator bool(Attr v)
        {
            string str = v.value;
            if (v.value != null)
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(Attr v)
        {
            string str = v.value;
            if (v.value != null)
            {
                short n;
                if (short.TryParse(str, out n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator int(Attr v)
        {
            string str = v.value;
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

        public static implicit operator long(Attr v)
        {
            string str = v.value;
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

        public static implicit operator decimal(Attr v)
        {
            string str = v.value;
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

        public static implicit operator Number(Attr v)
        {
            return default(Number);
        }

        public static implicit operator DateTime(Attr v)
        {
            return default(DateTime);
        }

        public static implicit operator NpgsqlPoint(Attr v)
        {
            return default(NpgsqlPoint);
        }

        public static implicit operator char[] (Attr v)
        {
            string str = v.value;
            return str?.ToCharArray();
        }

        public static implicit operator string(Attr v)
        {
            return v.value;
        }

        public static implicit operator byte[] (Attr v)
        {
            return null;
        }

        public static implicit operator short[] (Attr v)
        {
            return null;
        }

        public static implicit operator int[] (Attr v)
        {
            return null;
        }

        public static implicit operator long[] (Attr v)
        {
            return null;
        }

        public static implicit operator string[] (Attr v)
        {
            return null;
        }

    }

}