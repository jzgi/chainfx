namespace Greatbone.Core
{
    /// <summary>
    /// A resolved node of the work hierarchy pertaining to a URI segment.
    /// </summary>
    public struct Seg
    {
        // as uri segment
        readonly string key;

        // key object get from principal
        readonly object princi;

        readonly Work work;

        internal Seg(Work work, string key, object princi)
        {
            this.key = key;
            this.princi = princi;
            this.work = work;
        }

        public string Key => key;

        public object Princi => princi;

        public Work Work => work;

        //
        // CONVERSION
        //

        public static implicit operator short(Seg v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (short) v.princi;
            if (short.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator int(Seg v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (int) v.princi;
            if (int.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator long(Seg v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (long) v.princi;
            if (long.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator string(Seg v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (string) v.princi;
            return str;
        }

        public static implicit operator short[](Seg v)
        {
            return (short[]) v.princi;
        }

        public static implicit operator int[](Seg v)
        {
            return (int[]) v.princi;
        }

        public static implicit operator long[](Seg v)
        {
            return (long[]) v.princi;
        }

        public static implicit operator string[](Seg v)
        {
            return (string[]) v.princi;
        }
    }
}