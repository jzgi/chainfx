namespace Greatbone.Core
{
    ///
    /// The bit-wise flags that filter what selectos data input/output.
    ///
    public static class Selector
    {
        public const ushort

            NONE = 0,

            ALL = 0xffff,

            // auto generated or with default
            AUTO = 0x8000,

            // binary
            BIN = 0x4000,

            // late-handled
            LATE = 0x2000,

            // many
            SUB = 0x1000,

            // hidden or reserved
            KEPT = 0x0800;


        public static bool Y(this ushort sel, ushort v)
        {
            return (sel & v) == v;
        }

        public static bool N(this ushort sel, ushort v)
        {
            return (sel & v) != v;
        }

        public static bool Auto(this ushort sel)
        {
            return (sel & AUTO) != AUTO;
        }

        public static bool Bin(this ushort sel)
        {
            return (sel & BIN) != BIN;
        }

        public static bool Late(this ushort sel)
        {
            return (sel & LATE) != LATE;
        }

        public static bool Sub(this ushort sel)
        {
            return (sel & SUB) != SUB;
        }

        public static bool Kept(this ushort sel)
        {
            return (sel & KEPT) != KEPT;
        }
    }
}