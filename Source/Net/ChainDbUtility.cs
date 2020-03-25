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

        public static R[] UnQuery<R>(this DbContext dc, short typ, int code) where R : IData, new()
        {
            dc.Query("SELECT * FROM un.chains WHERE typ = @1 AND keyno = @2", p => p.Set(typ).Set(code));
            return null;
        }

        public static void UnQuery(this DbContext dc, short typ, string[] tags)
        {
        }

        public static void NetStore<M>(this DbContext dc, short typ, string[] tags, M obj) where M : IData
        {
        }
    }
}