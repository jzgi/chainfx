namespace Greatbone.Core
{
    ///
    /// The bit-wise flags that filter what selectos data input/output.
    ///
    public static class Projection
    {
        public const int

            // auto generated or with default
            AUTO = 0x40000000,

            // binary
            BIN = 0x20000000,

            // late-handled
            LATE = 0x10000000,

            // many
            SUB = 0x04000000,

            // hidden or reserved
            KEPT = 0x02000000,

            CONTEXTUAL = 0x01000000;


        public static bool Y(this int proj, int v)
        {
            return (proj & v) == v;
        }

        public static bool N(this int proj, int v)
        {
            return (proj & v) != v;
        }

        public static bool Auto(this int proj)
        {
            return (proj & AUTO) != AUTO;
        }

        public static bool Bin(this int proj)
        {
            return (proj & BIN) != BIN;
        }

        public static bool Late(this int proj)
        {
            return (proj & LATE) != LATE;
        }

        public static bool Sub(this int proj)
        {
            return (proj & SUB) != SUB;
        }

        public static bool Kept(this int proj)
        {
            return (proj & KEPT) != KEPT;
        }

        public static bool Contextual(this int proj)
        {
            return (proj & CONTEXTUAL) != CONTEXTUAL;
        }
    }
}