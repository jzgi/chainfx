using System;
using System.Threading.Tasks;
using SkyChain.Db;

namespace SkyChain.Chain
{
    public static class ChainUtility
    {
        public static async Task<bool> ChainStartTran(this DbContext dc, string an, short typ, string inst, string descr, decimal amt, JObj doc = null, string npeerid = null, string nan = null)
        {
            // if exists
            var def = ChainEnv.GetDefinition(typ);
            if (def == null)
            {
                throw new ChainException("definition not found: typ = " + typ);
            }
            var act = def.StartActivity;

            // padded sequence number
            var tn = (string) dc.Scalar("SELECT lpad(to_hex(nextval('chain.txn')),8, '0')");
            tn = ChainEnv.Info.id + tn;

            var cc = new ChainContext();

            var op = new Operation()
            {
                tn = tn,
                step = act.step,
                an = an,
                typ = typ,
                inst = inst,
                descr = descr,
                doc = doc,
                stamp = DateTime.Now,
                npeerid = npeerid,
                nan = nan
            };

            var ok = act.OnSubmit(cc, dc);
            if (!ok)
            {
                return false;
                // throw new ChainException("input error: tn = " + op.tn + ", step = " + op.step);
            }

            // data access
            dc.Sql("INSERT INTO ops ").colset(op)._VALUES_(op);
            await dc.ExecuteAsync(p => op.Write(p));

            return true;
        }

        internal static Record[] ChainGetTranaction(this DbContext dc, string tn)
        {
            var recs = dc.Query<Record>("SELECT * FROM chain.blockrecs WHERE tn = @1 ORDER BY step", p => p.Set(tn));
            return recs;
        }

        public static Record[] ChainGetTrace(this DbContext dc, short typ, string an, string inst = null)
        {
            var recs = dc.Query<Record>("SELECT * FROM chain.blockrecs WHERE typ = @1 AND an = @2 AND inst = @3 ORDER BY stemp DESC", p => p.Set(typ).Set(an).Set(inst));
            return recs;
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