
namespace Greatbone.Core
{
    public static class UShortkUtility
    {
        public const ushort Select = 0x8000;

        public const ushort Update = 0x4000;

        public const ushort Insert = 0x2000;

        public const ushort InsertOrUpdate = Insert | Update;


        public static bool Has(this ushort x, ushort v)
        {
            return (x & v) == v;
        }

        public static bool Sel(this ushort x)
        {
            return (x & Select) == Select;
        }

        public static bool Ins(this ushort x)
        {
            return (x & Insert) == Insert;
        }

        public static bool Upd(this ushort x)
        {
            return (x & Update) == Update;
        }

        public static bool InsOrUpd(this ushort x)
        {
            return (x & InsertOrUpdate) == InsertOrUpdate;
        }


    }
}