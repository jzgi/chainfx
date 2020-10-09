using System;
using Skyiah.Db;

namespace Skyiah.Chain
{
    public static class ChainDbUtility
    {
        public static bool ChainOpen(this DbContext dc, short typ, string key, int amt, byte[] attach)
        {
            // if exists
            dc.Sql("SELECT * FROM chain.recs WHERE typ = 1 AND key = @1");
            dc.Sql("SELECT * FROM chain.blocklns WHERE nodeid = '&' AND typ = 1 AND key = @1");


            dc.Sql("INSERT INTO chain.transacts (code, typ, amt, attach, lnodeid, lkey, lbalance) VALUES ()");
            return true;
        }

        public static bool ChainClose(this DbContext dc, short typ, string key, int amt, byte[] attach)
        {
            // if exists
            dc.Sql("SELECT * FROM chain.transacts WHERE typ = 1 AND key = @1");
            dc.Sql("SELECT * FROM chain.blocklns WHERE nodeid = '&' AND typ = 1 AND key = @1");


            dc.Sql("INSERT INTO chain.transacts (code, typ, amt, attach, lnodeid, lkey, lbalance) VALUES ()");
            return true;
        }

        public static bool ChainAdjust(this DbContext dc, short typ, string key, int amt, byte[] attach)
        {
            // if exists
            dc.Sql("SELECT * FROM chain.transacts WHERE typ = 1 AND key = @1");
            dc.Sql("SELECT * FROM chain.blocklns WHERE nodeid = '&' AND typ = 1 AND key = @1");


            dc.Sql("INSERT INTO chain.transacts (code, typ, amt, attach, lnodeid, lkey, lbalance) VALUES ()");
            return true;
        }

        /// <summary>
        /// Migrate a remote account to the local node. 
        /// </summary>
        public static bool ChainMigrate(this DbContext dc, short typ, string key, int amt, byte[] attach)
        {
            // if exists
            dc.Sql("SELECT * FROM chain.transacts WHERE typ = 1 AND key = @1");
            dc.Sql("SELECT * FROM chain.blocklns WHERE nodeid = '&' AND typ = 1 AND key = @1");


            dc.Sql("INSERT INTO chain.transacts (code, typ, amt, attach, lnodeid, lkey, lbalance) VALUES ()");
            return true;
        }

        public static bool ChainTransfer(this DbContext dc, short typ, string key, int amt, byte[] attach)
        {
            // if exists
            dc.Sql("SELECT * FROM chain.transacts WHERE typ = 1 AND key = @1");
            dc.Sql("SELECT * FROM chain.blocklns WHERE nodeid = '&' AND typ = 1 AND key = @1");


            dc.Sql("INSERT INTO chain.transacts (code, typ, amt, attach, lnodeid, lkey, lbalance) VALUES ()");
            return true;
        }


        internal static Block[] ChainGetBlock(this DbContext dc, short typid, int code)
        {
            dc.Query("SELECT * FROM chain.blocks WHERE typid = @1 AND key = @2", p => p.Set(typid).Set(code));
            return null;
        }

        public static byte[] ChainQuery(this DbContext dc, short typ, string an)
        {
            dc.Sql("SELECT body FROM chain.blocks WHERE typid = @1 AND key = @2");
            dc.Query(p => p.Set(typ).Set(an));
            dc.Let(out byte[] body);

            return default;
        }

        public static (int amt, int balance, DateTime stamp) ChainGet(this DbContext dc, short typ, string key, string nodeid = "&")
        {
            // var typs = Framework.Obtain<Map<short, Typ>>();

            dc.Sql("SELECT body FROM chain.blocks WHERE typid = @1 AND key = @2");
            dc.Query(p => p.Set(typ).Set(key));

            // var typ = typs[typid];
            // if (typ == null) return null;
            // if (typ.op <= 1)
            // {
            //     return null;
            // }
            //
            // var cryptokey = typ.op >= 3 ? Framework.publickey : Framework.privatekey;
            //
            // while (dc.Next())
            // {
            //     dc.Let(out byte[] body);
            //     // descrypt
            //     CryptionUtility.Decrypt(body, body.Length, cryptokey);
            //
            //     var jc = new JsonParser(body, body.Length).Parse();
            // }
            return (0, 0, DateTime.Now);
        }
    }
}