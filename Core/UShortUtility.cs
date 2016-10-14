
namespace Greatbone.Core
{
    public static class UShortkUtility
    {
        public const ushort SEL = 0x8000;

        public const ushort UPD = 0x4000;

        public const ushort INS = 0x2000;

        public const ushort INS_OR_UPD = INS | UPD;


        public static bool Has(this ushort x, ushort v)
        {
            return (x & v) == v;
        }

        public static bool Sel(this ushort x)
        {
            return (x & SEL) == SEL;
        }

        public static bool Ins(this ushort x)
        {
            return (x & INS) == INS;
        }

        public static bool Upd(this ushort x)
        {
            return (x & UPD) == UPD;
        }

        public static bool InsOrUpd(this ushort x)
        {
            return (x & INS_OR_UPD) == INS_OR_UPD;
        }


    }
}