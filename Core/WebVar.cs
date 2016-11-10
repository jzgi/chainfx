using System;

namespace Greatbone.Core
{
    public struct WebVar
    {
        readonly WebDir dir;

        readonly string key;

        internal WebVar(WebDir dir, string key)
        {
            this.dir = dir;
            this.key = key;
        }

        public WebDir Dir => dir;

        public string Key => key;

        //
        // CONVERSION
        //

        public static implicit operator bool(WebVar v)
        {
            string str = v.key;
            if (str != null)
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(WebVar v)
        {
            string str = v.key;
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

        public static implicit operator int(WebVar v)
        {
            string str = v.key;
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

        public static implicit operator long(WebVar v)
        {
            string str = v.key;
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

        public static implicit operator decimal(WebVar v)
        {
            string str = v.key;
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

        public static implicit operator Number(WebVar v)
        {
            return default(Number);
        }

        public static implicit operator DateTime(WebVar v)
        {
            return default(DateTime);
        }

        public static implicit operator char[](WebVar v)
        {
            string str = v.key;
            return str?.ToCharArray();
        }

        public static implicit operator string(WebVar v)
        {
            return v.key;
        }
    }
}