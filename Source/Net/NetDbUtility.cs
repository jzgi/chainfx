using CloudUn.Db;

namespace CloudUn.Net
{
    public static class NetDbUtility
    {
        static NetDbUtility()
        {
            using var dc = Framework.NewDbContext();
            // DDL
        }

        public static R[] ChainQuery<R>(this DbContext dc, short typ, int code) where R : IData, new()
        {
            return dc.Query<R>("SELECT * FROM un.chain WHERE typ = @1 AND code = @2", p => p.Set(typ).Set(code));
        }

        public static void NetQuery(this DbContext dc, short typ, string[] tags)
        {
        }

        public static void NetStore<M>(this DbContext dc, short typ, string[] tags, M obj) where M : IData
        {
        }
    }
}