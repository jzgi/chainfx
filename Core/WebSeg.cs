using System;

namespace Greatbone.Core
{
    ///
    /// A resolved variable part or segment in the URI path.
    ///
    public struct WebSeg
    {
        readonly string key;

        readonly WebFolder folder;

        internal WebSeg(string key, WebFolder folder)
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

        public static implicit operator bool(WebSeg v)
        {
            string str = v.key;
            if (!string.IsNullOrEmpty(str))
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(WebSeg v)
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

        public static implicit operator int(WebSeg v)
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

        public static implicit operator long(WebSeg v)
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

        public static implicit operator decimal(WebSeg v)
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

        public static implicit operator DateTime(WebSeg v)
        {
            return default(DateTime);
        }

        public static implicit operator char[] (WebSeg v)
        {
            string str = v.key;
            return str?.ToCharArray();
        }

        public static implicit operator string(WebSeg v)
        {
            return v.key;
        }
    }
}