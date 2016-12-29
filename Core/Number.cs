namespace Greatbone.Core
{
    ///
    /// Used to build a number during a parsing process.
    ///
    public struct Number
    {
        static readonly int[] INT =
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

        internal Number(int first)
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
            get { return pt >= 0; }
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

        internal double Double => (double)Decimal;

        internal long Long => negative ? -bigint : bigint;

        internal int Int => negative ? (int)-bigint : (int)bigint;

        internal short Short => negative ? (short)-bigint : (short)bigint;

        internal decimal Decimal
        {
            get
            {
                if (pt <= 0) return new decimal(bigint);

                int bits = Pow(fract);
                long v = bigint * INT[pt] + fract;
                int lo = (int)v;
                int mid = (int)(v >> 32);
                byte scale = (byte)pt;

                return new decimal(lo, mid, 0, negative, scale);
            }
        }

        static int Pow(int n)
        {
            int i = 0;
            int sum = 1;
            while (sum < n)
            {
                sum <<= 1;
                i++;
            }
            return i;
        }

        // sparse bit count
        static int Bits(int n)
        {
            int i = 0;
            while (n != 0)
            {
                i++;
                n &= (n - 1);
            }
            return i;
        }
    }
}