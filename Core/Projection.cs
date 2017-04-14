namespace Greatbone.Core
{
    ///
    /// The bit-wise flags that filter what selectos data input/output.
    ///
    public static class Projection
    {
        public const int

            // non-data or for control
            CTRL = 0x40000000,

            // primary or key
            PRIME = 0x08000000,

            // auto generated or with default
            AUTO = 0x04000000,

            // binary
            BIN = 0x02000000,

            // late-handled
            LATE = 0x01000000,

            // many
            DETAIL = 0x00800000,

            // transform or digest
            TRANSF = 0x00400000,

            // secret or protected
            SECRET = 0x00200000,

            // need authority
            POWER = 0x00100000,

            // frozen or immutable
            IMMUT = 0x00080000,

            // flow phases
            PHASE_A = 0x00008000,
            PHASE_B = 0x00004000,
            PHASE_C = 0x00002000,
            PHASE_D = 0x00001000;


        public static bool Y(this int proj, int v)
        {
            return (proj & v) == v;
        }

        public static bool N(this int proj, int v)
        {
            return (proj & v) != v;
        }

        public static bool Ctrl(this int proj)
        {
            return (proj & CTRL) == CTRL;
        }

        public static bool Prime(this int proj)
        {
            return (proj & PRIME) == PRIME;
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

        public static bool PhaseA(this int proj)
        {
            return (proj & PHASE_A) == PHASE_A;
        }

        public static bool PhaseB(this int proj)
        {
            return (proj & PHASE_B) == PHASE_B;
        }

        public static bool PhaseC(this int proj)
        {
            return (proj & PHASE_C) == PHASE_C;
        }

        public static bool PhaseD(this int proj)
        {
            return (proj & PHASE_D) == PHASE_D;
        }
    }
}