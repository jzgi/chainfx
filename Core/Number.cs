namespace Greatbone.Core
{
    /// <summary>
    /// Used to build a number during parsing process.
    /// </summary>
    internal struct Number
    {
        // the integral part
        long integr;

        // the fraction part
        int fract;

        // point & the scaling factor
        byte pt;

        // negative
        bool negat;

        internal Number(byte b)
        {
            integr = 0;
            fract = 0;
            pt = 0; // without point yet
            if (b == '-')
            {
                negat = true;
            }
            else
            {
                negat = false;
                Add(b);
            }
        }

        internal void Add(byte b)
        {
            if (b == '.')
            {
                pt = 0;
            }
            else
            {
                int n = b - '0';
                if (pt >= 0)
                {
                    fract = fract * 10 + n;
                    pt++;
                }
                else { integr = integr * 10 + n; }
            }
        }

        internal long Int64 => negat ? -integr : integr;

        internal int Int32 => negat ? (int)-integr : (int)integr;

        internal short Int16 => negat ? (short)-integr : (short)integr;

        internal decimal Decimal
        {
            get
            {
                int bits = Bits(fract);
                int lo = (int)(integr << bits) | fract;
                int mid = (int)(integr >> (32 - bits));
                int hi = (int)(integr >> (64 - bits));
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