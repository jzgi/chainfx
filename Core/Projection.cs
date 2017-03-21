namespace Greatbone.Core
{
    ///
    /// The bit-wise flags that filter what selectos data input/output.
    ///
    public static class Projection
    {
        public const int

            // primary or key
            PRIME = 0x40000000,

            // auto generated or with default
            AUTO = 0x20000000,

            // binary
            BIN = 0x10000000,

            // late-handled
            LATE = 0x08000000,

            // many
            DETAIL = 0x04000000,

            // transform or digest
            TRANSF = 0x02000000,

            // secret or protected
            SECRET = 0x01000000,

            /// need authority
            POWER = 0x00800000,

            /// frozen or immutable
            IMMUT = 0x00400000,

            // non-data or for control
            CTRL = 0x00200000;


        public static bool Y(this int proj, int v)
        {
            return (proj & v) == v;
        }

        public static bool N(this int proj, int v)
        {
            return (proj & v) != v;
        }

        public static bool Prime(this int proj)
        {
            return (proj & PRIME) == PRIME;
        }

        public static bool Auto(this int proj)
        {
            return (proj & AUTO) == AUTO;
        }

        public static bool AutoPrime(this int proj)
        {
            return (proj & (AUTO | PRIME)) == (AUTO | PRIME);
        }

        public static bool Bin(this int proj)
        {
            return (proj & BIN) == BIN;
        }

        public static bool Late(this int proj)
        {
            return (proj & LATE) == LATE;
        }

        public static bool Detail(this int proj)
        {
            return (proj & DETAIL) == DETAIL;
        }

        public static bool Transf(this int proj)
        {
            return (proj & TRANSF) == TRANSF;
        }

        public static bool Secret(this int proj)
        {
            return (proj & SECRET) == SECRET;
        }

        public static bool Power(this int proj)
        {
            return (proj & POWER) == POWER;
        }

        public static bool Immut(this int proj)
        {
            return (proj & IMMUT) == IMMUT;
        }
    }
}