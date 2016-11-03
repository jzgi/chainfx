
namespace Greatbone.Core
{

    public static class XUtility
    {

        // BASE = 0

        // ALL = 0xff

        public const byte

            // auto generated or with default
            AUTO = 0x80,

            // binary
            BIN = 0x40,

            // late-handled
            LATE = 0x20,

            // many
            DEEP = 0x10,

            // reserved
            RESV = 0x08;


        public static bool Ya(this byte x, byte v)
        {
            return (x & v) == v;
        }

        public static bool No(this byte x, byte v)
        {
            return (x & v) != v;
        }

    }

}