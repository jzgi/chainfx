
namespace Greatbone.Core
{
    public static class XUtility
    {

        public const uint

            XDefault = 0x80000000,

            XBinary = 0x40000000,

            XExtra = 0x20000000;


        public static bool Has(this uint x, uint v)
        {
            return (x & v) == v;
        }

        public static bool No(this uint x, uint v)
        {
            return (x & v) != v;
        }

        public static bool Both(this uint x, uint v1, uint v2)
        {
            return (x & v1) == v1 && (x & v2) == v2;
        }

        public static bool Either(this uint x, uint v1, uint v2)
        {
            return (x & v1) == v1 || (x & v2) == v2;
        }

        public static bool Neither(this uint x, uint v1, uint v2)
        {
            return (x & v1) != v1 && (x & v2) != v2;
        }

        //
        // CONVENIENT
        //

        public static bool DefaultOn(this uint x)
        {
            return x.Has(XDefault);
        }

        public static bool BinaryOn(this uint x)
        {
            return x.Has(XBinary);
        }

        public static bool ExtraOn(this uint x)
        {
            return x.Has(XExtra);
        }

    }
}