using CloudUn.Db;

namespace CloudUn.Net
{
    public static class ChainDbUtility
    {
        static ChainDbUtility()
        {
            using var dc = Framework.NewDbContext();
            // DDL


            // 
        }

        public static R[] ChainQuery<R>(this DbContext dc, short typ, int code) where R : IData, new()
        {
            dc.Query("SELECT * FROM un.blocks WHERE typ = @1 AND keyno = @2", p => p.Set(typ).Set(code));
            return null;
        }

        internal static Block[] ChainGetBlock(this DbContext dc, short typ, int code)
        {
            dc.Query("SELECT * FROM un.blocks WHERE typ = @1 AND keyno = @2", p => p.Set(typ).Set(code));
            return null;
        }

        public static void UnQuery(this DbContext dc, short typ, string[] tags)
        {
        }

        public static void NetStore<M>(this DbContext dc, short typ, string[] tags, M obj) where M : IData
        {
        }

        public static void ChainPut(this DbContext dc, short typ, string[] tags, Block obj)
        {
        }
    }
}