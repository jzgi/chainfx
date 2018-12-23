namespace Greatbone.Service
{
    /// <summary>
    /// A resolution segment along the uri path in the work hierarchy
    /// </summary>
    public struct Segment
    {
        // as uri segment
        readonly string key;

        // state object get from principal
        readonly object accessor;

        readonly Work work;

        internal Segment(Work work, string key, object accessor)
        {
            this.key = key;
            this.accessor = accessor;
            this.work = work;
        }

        public Work Work => work;

        public string Key => key;

        public object Accessor => accessor;

        //
        // CONVERSION
        //

        public static implicit operator short(Segment v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (short) v.accessor;
            if (short.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator int(Segment v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (int) v.accessor;
            if (int.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator long(Segment v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (long) v.accessor;
            if (long.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator string(Segment v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (string) v.accessor;
            return str;
        }

        public static implicit operator short[](Segment v)
        {
            return (short[]) v.accessor;
        }

        public static implicit operator int[](Segment v)
        {
            return (int[]) v.accessor;
        }

        public static implicit operator long[](Segment v)
        {
            return (long[]) v.accessor;
        }

        public static implicit operator string[](Segment v)
        {
            return (string[]) v.accessor;
        }
    }
}