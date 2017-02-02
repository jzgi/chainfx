namespace Greatbone.Core
{
    ///
    /// The bit-wise flags that filter what selectos data input/output.
    ///
    public static class Projection
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


        public static bool Y(this ushort proj, ushort v)
        {
            return (proj & v) == v;
        }

        public static bool N(this ushort proj, ushort v)
        {
            return (proj & v) != v;
        }

        public static bool Auto(this ushort proj)
        {
            return (proj & AUTO) != AUTO;
        }

        public static bool Bin(this ushort proj)
        {
            return (proj & BIN) != BIN;
        }

        public static bool Late(this ushort proj)
        {
            return (proj & LATE) != LATE;
        }

        public static bool Sub(this ushort proj)
        {
            return (proj & SUB) != SUB;
        }

        public static bool Kept(this ushort proj)
        {
            return (proj & KEPT) != KEPT;
        }
    }
}