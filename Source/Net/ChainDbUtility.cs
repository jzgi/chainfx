using CloudUn.Db;

namespace CloudUn.Net
{
    public static class ChainDbUtility
    {
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

        public static M[] ChainQuery<M>(this DbContext dc, short typid, string key) where M : IData
        {
            dc.Sql("SELECT * FROM chain.blocks WHERE typid = @1 AND key = @2");
            return default;
        }

        public static M[] ChainGet<M>(this DbContext dc, short typid, string[] tags) where M : IData
        {
            dc.Sql("SELECT * FROM chain.blocks WHERE typid = @1 AND ");
            return null;
        }

        public static void ChainPut(this DbContext dc, short typid, string key, string[] tags, byte[] data)
        {
            // retrieve prior hash

            // calculate new hash based on prior hash and the content
            byte[] content = data; // encrypt
            var prior = (string) dc.Scalar("SELECT hash FROM chain.blocks WHERE typid = @1 ORDER BY seq DESC LIMIT 1", p => p.Set(typid));
            string hash = "" + prior;

            // record insertion
            dc.Sql("INSERT INTO chain.blocks (typid, key, tags, content, hash) VALUES (@1, @2, @3, @4, @5)");
            dc.Execute(p => p.Set(typid).Set(key).Set(tags).Set(content).Set(hash));
        }
    }
}