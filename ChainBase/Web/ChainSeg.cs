using System;

namespace ChainBase.Web
{
    /// <summary>
    /// A resolution segment along the uri path in the work hierarchy
    /// </summary>
    public struct ChainSeg
    {
        // as uri segment
        readonly string key;

        // state object get from principal
        readonly object accessor;

        readonly WebWork work;

        internal ChainSeg(WebWork work, string key, object accessor)
        {
            this.key = key;
            this.accessor = accessor;
            this.work = work;
        }

        public WebWork Work => work;

        public string Key => key;

        public object Accessor => accessor;

        public bool IsImplicit => key.Length == 0;

        //
        // CONVERSION
        //

        public static implicit operator short(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str)) return (short) seg.accessor;
            if (short.TryParse(str, out var v))
            {
                return v;
            }

            return 0;
        }

        public static implicit operator int(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str)) return (int) seg.accessor;
            if (int.TryParse(str, out var v))
            {
                return v;
            }

            return 0;
        }

        public static implicit operator long(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str)) return (long) seg.accessor;
            if (long.TryParse(str, out var v))
            {
                return v;
            }

            return 0;
        }

        public static implicit operator string(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str)) return (string) seg.accessor;
            return str;
        }

        public static implicit operator ValueTuple<string, string>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, null);
            }

            return str.ToStringString();
        }

        public static implicit operator ValueTuple<string, short>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, 0);
            }

            return str.ToStringShort();
        }

        public static implicit operator ValueTuple<string, int>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, 0);
            }

            return str.ToStringInt();
        }

        public static implicit operator ValueTuple<string, long>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (null, 0);
            }

            return str.ToStringLong();
        }

        public static implicit operator ValueTuple<short, string>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, null);
            }

            return str.ToShortString();
        }

        public static implicit operator ValueTuple<short, short>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }
            return str.ToShortShort();
        }

        public static implicit operator ValueTuple<short, int>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }

            return str.ToShortInt();
        }

        public static implicit operator ValueTuple<short, long>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0L);
            }

            return str.ToShortLong();
        }

        public static implicit operator ValueTuple<int, string>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, null);
            }

            return str.ToIntString();
        }

        public static implicit operator ValueTuple<int, short>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }

            return str.ToIntShort();
        }

        public static implicit operator ValueTuple<int, int>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }

            return str.ToIntInt();
        }

        public static implicit operator ValueTuple<int, long>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }

            return str.ToIntLong();
        }

        public static implicit operator ValueTuple<long, string>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, null);
            }

            return str.ToLongString();
        }

        public static implicit operator ValueTuple<long, short>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }

            return str.ToLongShort();
        }

        public static implicit operator ValueTuple<long, int>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }

            return str.ToLongInt();
        }

        public static implicit operator ValueTuple<long, long>(ChainSeg seg)
        {
            string str = seg.key;
            if (string.IsNullOrEmpty(str))
            {
                return (0, 0);
            }

            return str.ToLongLong();
        }

        public static implicit operator string[](ChainSeg seg)
        {
            return (string[]) seg.accessor;
        }

        public static implicit operator short[](ChainSeg seg)
        {
            return (short[]) seg.accessor;
        }

        public static implicit operator int[](ChainSeg seg)
        {
            return (int[]) seg.accessor;
        }

        public static implicit operator long[](ChainSeg seg)
        {
            return (long[]) seg.accessor;
        }
    }
}