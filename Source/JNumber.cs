namespace Skyiah
{
    ///
    /// Used to build a number during a parsing process.
    ///
    public struct JNumber
    {
        static readonly int[] BASE =
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };

        // the integral part
        internal long bigint;

        // the fraction part
        internal int fract;

        // point & the scaling factor
        sbyte pt;

        // negative
        internal bool negative;

        internal JNumber(int first)
        {
            bigint = 0;
            fract = 0;
            pt = -1; // without point yet
            if (first == '-')
            {
                negative = true;
            }
            else
            {
                negative = false;
                Add(first);
            }
        }

        internal bool Pt
        {
            get => pt >= 0;
            set
            {
                if (value) pt = 0;
            }
        }

        internal void Add(int b)
        {
            int n = b - '0';
            if (pt >= 0)
            {
                fract = fract * 10 + n;
                pt++;
            }
            else
            {
                bigint = bigint * 10 + n;
            }
        }

        public double Double => (double) Decimal;

        public long Long => negative ? -bigint : bigint;

        public int Int => negative ? (int) -bigint : (int) bigint;

        public short Short => negative ? (short) -bigint : (short) bigint;

        public decimal Decimal
        {
            get
            {
                if (pt <= 0) return new decimal(bigint);

                long v = bigint * BASE[pt] + fract;
                int lo = (int) v;
                int mid = (int) (v >> 32);
                byte scale = (byte) pt;

                return new decimal(lo, mid, 0, negative, scale);
            }
        }

        public static implicit operator double(JNumber v) => v.Double;

        public static implicit operator long(JNumber v) => v.Long;

        public static implicit operator int(JNumber v) => v.Int;

        public static implicit operator short(JNumber v) => v.Short;

        public static implicit operator decimal(JNumber v) => v.Decimal;

        /// <summary>
        /// Cast from int.
        /// </summary>
        public static implicit operator JNumber(long v)
        {
            JNumber num;
            num.bigint = v;
            num.fract = 0;
            num.pt = 0;
            num.negative = v < 0;
            return num;
        }

        /// <summary>
        /// Cast from int.
        /// </summary>
        public static implicit operator JNumber(int v)
        {
            JNumber num;
            num.bigint = v;
            num.fract = 0;
            num.pt = 0;
            num.negative = v < 0;
            return num;
        }
    }
}