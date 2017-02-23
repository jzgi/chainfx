using System;

namespace Greatbone.Core
{
    ///
    /// A resolved part or segment in the URI path.
    ///
    public struct Segment
    {
        readonly string key;

        readonly Folder folder;

        internal Segment(string key, Folder folder)
        {
            this.key = key;
            this.folder = folder;
        }

        public string Key => key;

        public Type Type => folder?.GetType();

        public Folder Folder => folder;

        public bool IsVar => folder is IVar;

        //
        // CONVERSION
        //

        public static implicit operator bool(Segment v)
        {
            string str = v.key;
            if (!string.IsNullOrEmpty(str))
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(Segment v)
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

        public static implicit operator int(Segment v)
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

        public static implicit operator long(Segment v)
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

        public static implicit operator decimal(Segment v)
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

        public static implicit operator DateTime(Segment v)
        {
            return default(DateTime);
        }

        public static implicit operator char[] (Segment v)
        {
            string str = v.key;
            return str?.ToCharArray();
        }

        public static implicit operator string(Segment v)
        {
            return v.key;
        }
    }
}