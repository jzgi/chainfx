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
            DETAIL = 0x08000000,

            // hidden or reserved
            CODE = 0x04000000,

            // human interactive only
            HUMAN = 0x02000000,

            CONTEXTUAL = 0x01000000,

            // operate by authority
            CTRL = 0x00800000;


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
            return (proj & AUTO) == AUTO;
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

        public static bool Code(this int proj)
        {
            return (proj & CODE) == CODE;
        }

        public static bool Human(this int proj)
        {
            return (proj & HUMAN) == HUMAN;
        }

        public static bool Contextual(this int proj)
        {
            return (proj & CONTEXTUAL) == CONTEXTUAL;
        }

        public static bool Ctrl(this int proj)
        {
            return (proj & CTRL) == CTRL;
        }
    }
}