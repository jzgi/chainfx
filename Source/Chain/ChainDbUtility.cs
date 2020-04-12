using System;
using System.Data.Common;
using System.Net.Security;
using SkyCloud.Db;

namespace SkyCloud.Chain
{
    public static class ChainDbUtility
    {
        public static R[] ChainQuery<R>(this DbContext dc, short typid, string key) where R : IData, new()
        {
            dc.Query("SELECT * FROM chain.blocks WHERE typid = @1 AND key = @2", p => p.Set(typid).Set(key));
            return null;
        }

        internal static Block[] ChainGetBlock(this DbContext dc, short typid, int code)
        {
            dc.Query("SELECT * FROM chain.blocks WHERE typid = @1 AND key = @2", p => p.Set(typid).Set(code));
            return null;
        }

        public static byte[] ChainQueryDat<M>(this DbContext dc, short typid, string key) where M : IData
        {
            dc.Sql("SELECT body FROM chain.blocks WHERE typid = @1 AND key = @2");
            dc.Query(p => p.Set(typid).Set(key));
            dc.Let(out byte[] body);


            return default;
        }

        public static M[] ChainGet<M>(this DbContext dc, short typid, string key) where M : IData
        {
            dc.Sql("SELECT body FROM chain.blocks WHERE typid = @1 AND key = @2");
            dc.Query(p => p.Set(typid).Set(key));

            var datypes = Framework.Obtain<Map<short, Typ>>();
            var dattyp = datypes[typid];
            if (dattyp.op <= 1)
            {
                return null;
            }

            var cryptokey = dattyp.op >= 3 ? Framework.publickey : Framework.privatekey;

            while (dc.Next())
            {
                dc.Let(out byte[] body);
                // descrypt
                CryptionUtility.Decrypt(body, body.Length, cryptokey);

                var jc = new JsonParser(body, body.Length).Parse();
            }
            return null;
        }

        public static void ChainPut(this DbContext dc, short typid, string key, string[] tags, DynamicContent content)
        {
            // retrieve prior hash

            // calculate new hash based on prior hash and the content
            var datypes = Framework.Obtain<Map<short, Typ>>();
            var dattyp = datypes[typid];
            if (dattyp.op <= 1)
            {
                return;
            }

            var cryptokey = dattyp.op >= 3 ? Framework.publickey : Framework.privatekey;
            CryptionUtility.Encrypt(content.Buffer, content.Count, cryptokey); // encrypt
            var prior = (string) dc.Scalar("SELECT hash FROM chain.blocks WHERE typid = @1 ORDER BY seq DESC LIMIT 1", p => p.Set(typid));
            string hash = content.MD5(prior);
            var body = new ArraySegment<byte>(content.Buffer, 0, content.Count);

            // record insertion
            dc.Sql("INSERT INTO chain.blocks (typid, key, tags, body, hash) VALUES (@1, @2, @3, @4, @5)");
            dc.Execute(p => p.Set(typid).Set(key).Set(tags).Set(body).Set(hash));
        }
    }
}