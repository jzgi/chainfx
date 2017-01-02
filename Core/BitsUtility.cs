namespace Greatbone.Core
{
    ///
    /// The bits-flags that filter what to load or dump in persistance operations.
    ///
    public static class BitsUtility
    {
        // BASE = 0

        // ALL = 0xff

        public const byte

            // auto generated or with default
            AUTO = 0x80,

            // binary
            BINARY = 0x40,

            // late-handled
            LATER = 0x20,

            // many
            SUBLEVEL = 0x10,

            // reserved
            HIDDEN = 0x08;


        public static bool Has(this byte bits, byte v)
        {
            return (bits & v) == v;
        }

        public static bool No(this byte bits, byte v)
        {
            return (bits & v) != v;
        }
    }
}