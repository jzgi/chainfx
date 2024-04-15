using System;

namespace ChainFX.Web
{
    /// <summary>
    /// A resolution segment along the uri path in the work hierarchy
    /// </summary>
    public readonly struct WebSeg
    {
        // as in uri segment
        readonly string key;

        // state object resolved by principal
        readonly object accessor;

        // corresponding work object
        readonly WebWork work;

        readonly int adscript;

        internal WebSeg(WebWork work, string key, int adscript, object accessor)
        {
            this.key = key;
            this.adscript = adscript;
            this.accessor = accessor;
            this.work = work;
        }

        public WebWork Work => work;

        public string Key => key;

        public int Adscript => adscript;

        public object Accessor => accessor;

        public bool IsImplicit => string.IsNullOrEmpty(key);

        public T As<T>() where T : class => accessor as T;

        //
        // CONVERSION
        //

        public static implicit operator short(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str)) return (short)seg.accessor;
            if (short.TryParse(str, out var v))
            {
                return v;
            }
            return 0;
        }

        public static implicit operator int(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str)) return (int)seg.accessor;
            if (int.TryParse(str, out var v))
            {
                return v;
            }
            return 0;
        }

        public static implicit operator long(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str)) return (long)seg.accessor;
            if (long.TryParse(str, out var v))
            {
                return v;
            }
            return 0;
        }

        public static implicit operator string(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (string)seg.accessor;
            }
            return str;
        }

        public static implicit operator DateTime(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str)) return (DateTime)seg.accessor;
            if (DateTime.TryParse(str, out var v))
            {
                return v;
            }
            return default;
        }

        public static implicit operator ValueTuple<string, string>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, null);
            }
            int pos = 0;
            var a = str.ParseString(ref pos);
            var b = str.ParseString(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<string, short>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, 0);
            }
            int pos = 0;
            var a = str.ParseString(ref pos);
            var b = str.ParseShort(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<string, int>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, 0);
            }
            int pos = 0;
            var a = str.ParseString(ref pos);
            var b = str.ParseInt(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<string, long>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, 0);
            }
            int pos = 0;
            var a = str.ParseString(ref pos);
            var b = str.ParseLong(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<short, string>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, null);
            }
            int pos = 0;
            var a = str.ParseShort(ref pos);
            var b = str.ParseString(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<short, short>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseShort(ref pos);
            var b = str.ParseShort(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<short, int>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseShort(ref pos);
            var b = str.ParseInt(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<short, long>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0L);
            }
            int pos = 0;
            var a = str.ParseShort(ref pos);
            var b = str.ParseLong(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<int, string>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, null);
            }
            int pos = 0;
            var a = str.ParseInt(ref pos);
            var b = str.ParseString(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<int, short>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseInt(ref pos);
            var b = str.ParseShort(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<int, int>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseInt(ref pos);
            var b = str.ParseInt(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<int, long>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseInt(ref pos);
            var b = str.ParseShort(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<long, string>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, null);
            }
            int pos = 0;
            var a = str.ParseLong(ref pos);
            var b = str.ParseString(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<long, short>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseLong(ref pos);
            var b = str.ParseShort(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<long, int>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseLong(ref pos);
            var b = str.ParseInt(ref pos);
            return (a, b);
        }

        public static implicit operator ValueTuple<long, long>(WebSeg seg)
        {
            var str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            int pos = 0;
            var a = str.ParseLong(ref pos);
            var b = str.ParseLong(ref pos);
            return (a, b);
        }

        public static implicit operator string[](WebSeg seg)
        {
            return (string[])seg.accessor;
        }

        public static implicit operator short[](WebSeg seg)
        {
            return (short[])seg.accessor;
        }

        public static implicit operator int[](WebSeg seg)
        {
            return (int[])seg.accessor;
        }

        public static implicit operator long[](WebSeg seg)
        {
            return (long[])seg.accessor;
        }
    }
}