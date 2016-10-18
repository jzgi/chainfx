
namespace Greatbone.Core
{
    public static class UShortkUtility
    {
        public const ushort X_SEL = 0x8000;

        public const ushort X_UPD = 0x4000;

        public const ushort X_INS = 0x2000;


        public static bool If(this ushort x, ushort v)
        {
            return (x & v) == v;
        }

        public static bool SEL(this ushort x)
        {
            return (x & X_SEL) == X_SEL;
        }

        public static bool INS(this ushort x)
        {
            return (x & X_INS) == X_INS;
        }

        public static bool UPD(this ushort x)
        {
            return (x & X_UPD) == X_UPD;
        }

        public static ushort OrSEL(this ushort x)
        {
            return (ushort)(x | X_INS);
        }

        public static ushort OrINS(this ushort x)
        {
            return (ushort)(x | X_INS);
        }

        public static ushort OrUPD(this ushort x)
        {
            return (ushort)(x | X_UPD);
        }


    }
}