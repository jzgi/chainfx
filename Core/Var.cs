using System;

namespace Greatbone.Core
{
    ///
    /// A resolved variable part in the URI path.
    ///
    public struct Var
    {
        readonly string value;

        readonly WebFolder folder;

        internal Var(string value, WebFolder folder)
        {
            this.value = value;
            this.folder = folder;
        }

        public string Value => value;

        public bool Empty => value == null;

        public WebFolder Folder => folder;

        //
        // CONVERSION
        //

        public static implicit operator bool(Var v)
        {
            string str = v.value;
            if (!string.IsNullOrEmpty(str))
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(Var v)
        {
            string str = v.value;
            if (!string.IsNullOrEmpty(str))
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
            string str = v.value;
            if (!string.IsNullOrEmpty(str))
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
            string str = v.value;
            if (!string.IsNullOrEmpty(str))
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
            string str = v.value;
            if (!string.IsNullOrEmpty(str))
            {
                decimal n;
                if (decimal.TryParse(str, out n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator DateTime(Var v)
        {
            return default(DateTime);
        }

        public static implicit operator char[] (Var v)
        {
            string str = v.value;
            return str?.ToCharArray();
        }

        public static implicit operator string(Var v)
        {
            return v.value;
        }
    }
}