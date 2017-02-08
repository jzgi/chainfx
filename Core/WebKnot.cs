using System;

namespace Greatbone.Core
{
    ///
    /// A resolved variable part in the URI path.
    ///
    public struct WebKnot
    {
        readonly string key;

        readonly WebFolder folder;

        internal WebKnot(string key, WebFolder folder)
        {
            this.key = key;
            this.folder = folder;
        }

        public string Key => key;

        public Type Type => folder?.GetType();

        public WebFolder Folder => folder;

        public bool IsVar => folder is IVar;

        //
        // CONVERSION
        //

        public static implicit operator bool(WebKnot v)
        {
            string str = v.key;
            if (!string.IsNullOrEmpty(str))
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(WebKnot v)
        {
            string str = v.key;
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

        public static implicit operator int(WebKnot v)
        {
            string str = v.key;
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

        public static implicit operator long(WebKnot v)
        {
            string str = v.key;
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

        public static implicit operator decimal(WebKnot v)
        {
            string str = v.key;
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

        public static implicit operator DateTime(WebKnot v)
        {
            return default(DateTime);
        }

        public static implicit operator char[] (WebKnot v)
        {
            string str = v.key;
            return str?.ToCharArray();
        }

        public static implicit operator string(WebKnot v)
        {
            return v.key;
        }
    }
}