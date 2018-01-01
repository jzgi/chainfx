namespace Greatbone.Core
{
    /// <summary>
    /// A resolved node of the work hierarchy corresponding to a uri segment.
    /// </summary>
    public struct Node
    {
        // as uri segment
        readonly string key;

        // key object get from principal
        readonly object princi;

        readonly Work work;

        internal Node(Work work, string key, object princi)
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

        public static implicit operator short(Node v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (short) v.princi;
            if (short.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator int(Node v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (int) v.princi;
            if (int.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator long(Node v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (long) v.princi;
            if (long.TryParse(str, out var n))
            {
                return n;
            }
            return 0;
        }

        public static implicit operator string(Node v)
        {
            string str = v.key;
            if (string.IsNullOrEmpty(str)) return (string) v.princi;
            return str;
        }

        public static implicit operator short[](Node v)
        {
            return (short[]) v.princi;
        }

        public static implicit operator int[](Node v)
        {
            return (int[]) v.princi;
        }

        public static implicit operator long[](Node v)
        {
            return (long[]) v.princi;
        }

        public static implicit operator string[](Node v)
        {
            return (string[]) v.princi;
        }
    }
}