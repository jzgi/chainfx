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

        internal static State[] FindStatesByJob(this DbContext dc, string job)
        {
            dc.Sql("SELECT ").collst(State.Empty).T(" FROM chain.blocksts WHERE job = @1 ORDER BY step");
            var recs = dc.Query<State>(p => p.Set(job));
            return recs;
        }

        public static async Task<State[]> FindStatesAsync(this DbContext dc, string acct, string ldgr, int limit = 20, int page = 0)
        {
            dc.Sql("SELECT ").collst(State.Empty).T(" FROM chain.blocksts WHERE acct = @1 AND ldgr LIKE @2 ORDER BY stamp DESC LIMIT @3 OFFSET @3 * @4");
            return await dc.QueryAsync<State>(p => p.Set(acct).Set(ldgr + "%").Set(limit).Set(page));
        }

        internal static Log[] FindLogs(this DbContext dc, string job)
        {
            dc.Sql("SELECT ").collst(Log.Empty).T(" FROM chain.logs WHERE job = @1 ORDER BY step");
            var recs = dc.Query<Log>(p => p.Set(job));
            return recs;
        }

        public static async Task<Log[]> FindAllAsync(this DbContext dc, string acct, string ldgr)
        {
            dc.Sql("SELECT ").collst(Log.Empty).T(" FROM chain.logs WHERE acct = @1 AND ldgr LIKE @2");
            return await dc.QueryAsync<Log>(p => p.Set(acct).Set(ldgr + "%"));
        }

        public static async Task<Log> FindAsync(this DbContext dc, string job, short step)
        {
            dc.Sql("SELECT ").collst(Log.Empty).T(" FROM chain.logs WHERE job = @1 AND step = @2");
            return await dc.QueryTopAsync<Log>(p => p.Set(job).Set(step));
        }

        public static async Task<Log> StartAsync(this DbContext dc, string acct, string ldgr, string descr, decimal amt, JObj doc = null)
        {
            // resolve transaction number
            var jobseq = (int) dc.Scalar("SELECT nextval('chain.jobseq')");
            string job = ChainEnviron.Info.id + TextUtility.ToHex(jobseq);

            // insert
            var log = new Log()
            {
                job = job,
                step = 1,
                acct = acct,
                ldgr = ldgr,
                amt = amt,
                descr = descr,
                doc = doc,
                stamp = DateTime.Now,
                status = Log.CREATED
            };

            // data access
            dc.Sql("INSERT INTO ops ").colset(log, 0)._VALUES_(log, 0);
            await dc.ExecuteAsync(p => log.Write(p));

            // push to tne next step
            //
            if (log.IsLocal)
            {
                dc.Sql("INSERT INTO ops ").colset(log)._VALUES_(log);
                await dc.ExecuteAsync(p => log.Write(p));
            }

            return null;
        }

        public static async Task ForwardAsync(this DbContext dc, string job, short step, string npeer, string nacct, string nname)
        {
            // get the 
            dc.Sql("SELECT ").collst(Log.Empty).T(" FROM logs WHERE job = @1 AND step = @2");
            var o = dc.QueryTop<Log>(p => p.Set(job).Set(step));

            if (o.IsLocal)
            {
                dc.Sql("INSERT INTO ops").T(Log.FORTH)._VALUES_(Log.FORTH);
                await dc.ExecuteAsync(p => o.Write(p));
            }
            else // remote call
            {
                var cli = ChainEnviron.GetChainClient(o.ppeer);
                // cli.PostAsync("");
            }

            // update status
            dc.Sql("UPDATE ops SET status = ").T(Log.FORTH).T(" WHERE tn = @1 AND step = @2");
            await dc.ExecuteAsync();
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