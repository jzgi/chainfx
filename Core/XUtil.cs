
namespace Greatbone.Core
{

    public static class XUtil
    {

        public const byte

            // auto generated or with default
            AUTO = 0x80,

            // binary
            BIN = 0x40,

            // late-handled
            LATE = 0x20,

            // many
            MANY = 0x10,

            // reserved
            RESV = 0x08;


        public static bool On(this byte x, byte v)
        {
            return (x & v) == v;
        }

        public static bool Off(this byte x, byte v)
        {
            return (x & v) != v;
        }

    }
    
}