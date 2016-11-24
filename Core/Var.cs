using System;

namespace Greatbone.Core
{
    ///
    /// A resolved variable directory key in the URI path.
    ///
    public struct Var
    {
        readonly WebDirectory directory;

        readonly string key;

        internal Var(WebDirectory directory, string key)
        {
            this.directory = directory;
            this.key = key;
        }

        public WebDirectory Directory => directory;

        public string Key => key;

        //
        // CONVERSION
        //

        public static implicit operator bool(Var v)
        {
            string str = v.key;
            if (str != null)
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(Var v)
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

        public static implicit operator int(Var v)
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

        public static implicit operator long(Var v)
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

        public static implicit operator decimal(Var v)
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

        public static implicit operator Number(Var v)
        {
            return default(Number);
        }

        public static implicit operator DateTime(Var v)
        {
            return default(DateTime);
        }

        public static implicit operator char[] (Var v)
        {
            string str = v.key;
            return str?.ToCharArray();
        }

        public static implicit operator string(Var v)
        {
            return v.key;
        }
    }
}