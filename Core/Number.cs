namespace Greatbone.Core
{
    /// <summary>
    /// To build a number during parsing process.
    /// </summary>
    internal struct Number
    {
        // the integral part
        long integr;

        // the fraction part
        int fract;

        // digital point
        bool pt;

        // negative
        bool negat;

        internal Number(byte b)
        {
            integr = 0;
            fract = 0;
            pt = false;
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
                pt = true;
            }
            else
            {
                int n = b - '0';
                if (pt) { fract = fract * 10 + n; }
                else { integr = integr * 10 + n; }
            }
        }

        internal long Int64 => negat ? -integr : integr;

        internal int Int32 => negat ? (int)-integr : (int)integr;

        internal int Int16 => negat ? (short)-integr : (short)integr;

        internal decimal Decimal
        {
            get
            {
                int bits = Bits(fract);
                int lo = (int)(integr << bits) | fract;
                int mid = (int)(integr >> (32 - bits));
                int hi = (int)(integr >> (64 - bits));
                byte scale = Digits(fract);

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

        static byte Digits(int n)
        {
            int i = 1;
            while ((n /= 10) != 0)
            {
                i++;
            }
            return (byte)i;
        }
    }
}