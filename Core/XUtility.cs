
namespace Greatbone.Core
{
    public static class XUtility
    {

        public const byte

            // auto generated or default
            AUTO = 0x80,

            // binary
            BIN = 0x40,

            // extra
            EXTRA = 0x20;

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