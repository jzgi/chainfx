namespace Greatbone.Core
{
    ///
    /// The bit-wise flags that filter what selectos data input/output.
    ///
    public static class Flags
    {
        public const ushort

            NONE = 0,

            ALL = 0xffff,

            // auto generated or with default
            AUTO = 0x8000,

            // binary
            BINARY = 0x4000,

            // late-handled
            LATE = 0x2000,

            // many
            SUB = 0x1000,

            // hidden or reserved
            KEPT = 0x0800;


        public static bool Y(this ushort flags, ushort v)
        {
            return (flags & v) == v;
        }

        public static bool N(this ushort flags, ushort v)
        {
            return (flags & v) != v;
        }
    }
}