namespace Greatbone.Core
{
    ///
    /// Used to build a number during a parsing process.
    ///
    public struct Number
    {
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

        internal double Double => (double) Decimal;

        internal long Long => negative ? -bigint : bigint;

        internal int Int => negative ? (int) -bigint : (int) bigint;

        internal short Short => negative ? (short) -bigint : (short) bigint;

        internal decimal Decimal
        {
            get
            {
                int bits = Pow(fract);
                int lo = (int) ((bigint << bits) + fract);
                int mid = (int) (bigint >> (32 - bits));
                int hi = (int) (bigint >> (64 - bits));
                byte scale = (byte) (pt - 1);

                return new decimal(lo, mid, hi, negative, scale);
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