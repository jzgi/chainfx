
namespace Greatbone.Core
{
    public static class XUtility
    {

        public static bool Yes(this uint x, uint v)
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

    }
}