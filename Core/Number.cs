namespace Greatbone.Core
{

    ///
    /// <summary>
    /// Used to build a number during a parsing process.
    /// </summary>
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
        internal bool negat;

        internal Number(byte first)
        {
            bigint = 0;
            fract = 0;
            pt = -1; // without point yet
            if (first == '-')
            {
                negat = true;
            }
            else
            {
                negat = false;
                Add(first);
            }
        }

        internal bool Pt
        {
            get { return pt >= 0; }
            set { if (value) pt = 0; }
        }

        internal void Add(byte b)
        {
            int n = b - '0';
            if (pt >= 0)
            {
                fract = fract * 10 + n;
                pt++;
            }
            else { bigint = bigint * 10 + n; }
        }

        internal long Long => negat ? -bigint : bigint;

        internal int Int => negat ? (int)-bigint : (int)bigint;

        internal short Short => negat ? (short)-bigint : (short)bigint;

        internal decimal Decimal
        {
            get
            {
                int bits = Bits(fract);
                int lo = (int)(bigint << bits) | fract;
                int mid = (int)(bigint >> (32 - bits));
                int hi = (int)(bigint >> (64 - bits));
                byte scale = (byte)(pt - 1);

                return new decimal(lo, mid, hi, negat, scale);
            }
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