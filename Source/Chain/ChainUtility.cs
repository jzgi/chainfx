using System;
using System.Threading.Tasks;
using SkyChain.Db;

namespace SkyChain.Chain
{
    public static class ChainUtility
    {
        public static Peer[] FindPeers(this DbContext dc)
        {
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
            return dc.Query<Peer>();
        }

        internal static async Task<Record[]> RetrieveAsync(this DbContext dc, long job)
        {
            dc.Sql("SELECT ").collst(Record.Empty).T(" FROM chain.blockrcs WHERE job = @1 ORDER BY step");
            return await dc.QueryAsync<Record>(p => p.Set(job));
        }

        public static async Task<Map<long, Record>> RetrieveAsync(this DbContext dc, string acct, string ldgr, int limit = 20, int offset = 0)
        {
            var map = new Map<long, Record>();
            dc.Sql("SELECT ").collst(Record.Empty).T(" FROM chain.blockrcs WHERE acct = @1 AND ldgr LIKE @2 ORDER BY stated DESC LIMIT @3 OFFSET @3 * @4");
            await dc.QueryAsync(p => p.Set(acct).Set(ldgr + "%").Set(limit).Set(offset));
            Record o;
            while (dc.Next())
            {
                o = dc.ToObject<Record>();
                map.Add(o);
            }
            // query again to see any rest steps
            dc.Sql("SELECT ").collst(Record.Empty).T(" FROM chain.blockrcs WHERE acct = @1 AND ldgr LIKE @2 AND job = @3 AND step > @4 ORDER BY stated");


            return null;
        }

        public static async Task<Log[]> LoadByLdgrAsync(this DbContext dc, string ldgr)
        {
            dc.Sql("SELECT ").collst(Log.Empty).T(" FROM chain.logs WHERE ldgr LIKE @2");
            return await dc.QueryAsync<Log>(p => p.Set(ldgr + "%"));
        }

        public static async Task<Log[]> LoadAsync(this DbContext dc, string acct, string ldgr)
        {
            dc.Sql("SELECT ").collst(Log.Empty).T(" FROM chain.logs WHERE acct = @1 AND ldgr LIKE @2");
            return await dc.QueryAsync<Log>(p => p.Set(acct).Set(ldgr + "%"));
        }

        public static async Task<Log[]> FindByJobAsync(this DbContext dc, string job)
        {
            dc.Sql("SELECT ").collst(Log.Empty).T(" FROM chain.logs WHERE job = @1");
            return await dc.QueryAsync<Log>(p => p.Set(job));
        }

        public static async Task<Log> JobStartAsync(this DbContext dc, string acct, string name, string ldgr, string descr, decimal amt, JObj doc = null)
        {
            // resolve transaction number
            var jobseq = (long) dc.Scalar("SELECT nextval('chain.jobseq')");
            long job = ((long) ChainEnviron.Info.id << 32) + jobseq;

            // insert
            var log = new Log()
            {
                job = job,
                step = 1,
                acct = acct,
                name = name,
                ldgr = ldgr,
                amt = amt,
                descr = descr,
                doc = doc,
                stated = DateTime.Now,
                status = Log.CREATED
            };

            // data access
            dc.Sql("INSERT INTO chain.logs ").colset(log, 0)._VALUES_(log, 0);
            await dc.ExecuteAsync(p => log.Write(p));

            return log;
        }

        public static async Task ForwardAsync(this DbContext dc, long job, string npeer, string nacct, string nname, decimal amt, JObj doc = null)
        {
            if (!await dc.QueryTopAsync("SELECT step FROM chain.logs WHERE job = @1 ORDER BY step DESC", p => p.Set(job)))
            {
                throw new ChainException();
            }
            dc.Let(out short step);

            dc.Sql("UPDATE chain.logs SET status = ").T(Log.FORWARD).T(", npeer = @1, nacct = @2, nname = @3 WHERE job = @4 AND step = @5 RETURNING ldgr");
            await dc.QueryTopAsync(p => p.Set(npeer).Set(nacct).Set(nname).Set(job).Set(step));
            dc.Let(out string ldgr);

            var o = new Log()
            {
                job = job,
                step = (short) (step + 1),
                acct = nacct,
                name = nname,
                ldgr = ldgr,
                amt = amt,
                descr = ldgr,
                doc = doc,
                stated = DateTime.Now,
                status = Log.CREATED
            };
            if (npeer == null)
            {
                dc.Sql("INSERT INTO chain.logs ").colset(Log.Empty)._VALUES_(Log.Empty);
                await dc.ExecuteAsync(p => o.Write(p));
            }
            else // remote call
            {
                var cli = ChainEnviron.GetChainClient(o.ppeer);
                // cli.PostAsync("");
            }
        }

        public static async Task SetForwardAsync(this DbContext dc, string tn, short step, string descr, decimal amt, JObj doc = null)
        {
        }

        public static async Task BackwardAsync(this DbContext dc, string tn, short step, string descr, decimal amt, JObj doc = null)
        {
        }

        public static async Task SetBackwardAsync(this DbContext dc, string tn, short step, string descr, decimal amt, JObj doc = null)
        {
        }

        public static async Task AbortAsync(this DbContext dc, string fn, short step, string descr, decimal amt, JObj doc = null)
        {
        }

        public static async Task EndAsync(this DbContext dc, string tn, short step, string descr, decimal amt, JObj doc = null)
        {
        }
    }
}