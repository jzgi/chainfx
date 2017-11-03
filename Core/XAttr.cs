using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An XML attribute.
    /// </summary>
    public struct XAttr : IRollable
    {
        readonly string name;

        readonly string value;

        public string Name => name;

        internal XAttr(string name, string v)
        {
            this.name = name;
            value = v;
        }

        public string Value => value;

        public static implicit operator bool(XAttr v)
        {
            string str = v.value;
            if (v.value != null)
            {
                return "true".Equals(str) || "1".Equals(str) || "on".Equals(str);
            }
            return false;
        }

        public static implicit operator short(XAttr v)
        {
            string str = v.value;
            if (v.value != null)
            {
                if (short.TryParse(str, out var n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator int(XAttr v)
        {
            string str = v.value;
            if (str != null)
            {
                if (int.TryParse(str, out var n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator long(XAttr v)
        {
            string str = v.value;
            if (str != null)
            {
                if (long.TryParse(str, out var n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator double(XAttr v)
        {
            string str = v.value;
            if (str != null)
            {
                if (double.TryParse(str, out var n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator decimal(XAttr v)
        {
            string str = v.value;
            if (str != null)
            {
                if (decimal.TryParse(str, out var n))
                {
                    return n;
                }
            }
            return 0;
        }

        public static implicit operator DateTime(XAttr v)
        {
            return default;
        }

        public static implicit operator char[](XAttr v)
        {
            string str = v.value;
            return str?.ToCharArray();
        }

        public static implicit operator string(XAttr v)
        {
            return v.value;
        }

        public static implicit operator byte[](XAttr v)
        {
            return null;
        }

        public static implicit operator short[](XAttr v)
        {
            return null;
        }

        public static implicit operator int[](XAttr v)
        {
            return null;
        }

        public static implicit operator long[](XAttr v)
        {
            return null;
        }

        public static implicit operator string[](XAttr v)
        {
            return null;
        }
    }
}