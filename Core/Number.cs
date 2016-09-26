namespace Greatbone.Core
{
    internal struct Number
    {
        bool sign;

        int integer;

        bool dot;

        int fraction;


        internal void Add(byte c)
        {
            int d = c - '0';
            if (!dot)
            {
                integer = integer * 10 + d;
            }
        }

        internal long Int32 => integer;

        internal decimal Decimal => new decimal(0, 0, integer, sign, 0);
    }
}