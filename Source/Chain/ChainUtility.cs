using System;
using System.Threading.Tasks;
using SkyChain.Db;
using static SkyChain.Chain.ChainEnviron;

namespace SkyChain.Chain
{
    public static class ChainUtility
    {
        public const int BLOCK_CAPACITY = 1000;

        internal static (int, short) ResolveSeq(long seq)
        {
            var blockid = (int) (seq / BLOCK_CAPACITY);
            var idx = (short) (seq % BLOCK_CAPACITY);
            return (blockid, idx);
        }

        internal static long WeaveSeq(int blockid, short idx)
        {
            return (long) blockid * BLOCK_CAPACITY + idx;
        }


        public static Peer[] AllPeers(this DbContext dc)
        {
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
            return dc.Query<Peer>();
        }

        /// <summary>
        /// To retrieve all archived steps for the specified job, might across peers.
        /// </summary>
        public static async Task<FlowState[]> RetrieveAsync(this DbContext dc, long job)
        {
            dc.Sql("SELECT ").collst(FlowState.Empty).T(" FROM chain.blocks WHERE job = @1 ORDER BY step");
            return await dc.QueryAsync<FlowState>(p => p.Set(job));
        }

        public static async Task<FlowState> RetrieveAsync(this DbContext dc, string acct, string ldgr)
        {
            dc.Sql("SELECT ").collst(FlowState.Empty).T(" FROM chain.blocks WHERE acct = @1 AND ldgr = @2 ORDER BY seq DESC LIMIT 1");
            return await dc.QueryTopAsync<FlowState>(p => p.Set(acct).Set(ldgr));
        }

        /// <summary>
        /// To retrieve a page of archived records for the specified account & ledger. It may across peers.
        /// </summary>
        public static async Task<FlowState[]> RetrieveAsync(this DbContext dc, string acct, string ldgr, short step, int limit = 20, int page = 0)
        {
            dc.Sql("SELECT ").collst(FlowState.Empty).T(" FROM chain.blocks WHERE peerid = @1 AND acct = @2 AND ldgr = @3 AND step = @4 ORDER BY seq DESC LIMIT @5 OFFSET @5 * @6");
            return await dc.QueryAsync<FlowState>(p => p.Set(Info.id).Set(acct).Set(ldgr).Set(step).Set(limit).Set(page));
        }

        public static async Task<FlowOp[]> GrabAsync(this DbContext dc, string acct, string ldgr, short step, short status = -1)
        {
            dc.Sql("SELECT ").collst(FlowOp.Empty).T(" FROM chain.ops WHERE acct = @1 AND ldgr = @2 AND step = @3").T(" AND status = @4", status > -1).T(" ORDER BY job DESC");
            return await dc.QueryAsync<FlowOp>(p => p.Set(acct).Set(ldgr).Set(step).Set(status));
        }

        public static async Task<FlowOp> GrabAsync(this DbContext dc, long job, short step)
        {
            dc.Sql("SELECT ").collst(FlowOp.Empty).T(" FROM chain.ops WHERE job = @1 AND step = @2");
            return await dc.QueryTopAsync<FlowOp>(p => p.Set(job).Set(step));
        }

        public static async Task<FlowOp[]> GrabAsync(this DbContext dc, string acct, string ldgr)
        {
            dc.Sql("SELECT ").collst(FlowOp.Empty).T(" FROM chain.ops WHERE acct = @1 AND ldgr LIKE @2");
            return await dc.QueryAsync<FlowOp>(p => p.Set(acct).Set(ldgr + "%"));
        }

        public static async Task<FlowOp[]> GrabAsync(this DbContext dc, long job)
        {
            dc.Sql("SELECT ").collst(FlowOp.Empty).T(" FROM chain.ops WHERE job = @1 ORDER BY step");
            return await dc.QueryAsync<FlowOp>(p => p.Set(job));
        }

        /// <summary>
        /// To start a job and create its first step.
        /// </summary>
        /// <returns></returns>
        public static async Task<long> FlowStartAsync(this DbContext dc, string acct, string name, string ldgr, string descr, decimal amt, JObj doc = null)
        {
            // job number is unique
            await dc.QueryTopAsync("SELECT nextval('chain.jobseq')");
            dc.Let(out int jobseq);
            long job = ((long) Info.id << 32) + jobseq;

            // insert
            var op = new FlowOp()
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
                status = FlowOp.STATUS_STARTED
            };
            dc.Sql("INSERT INTO chain.ops ").colset(op, 0)._VALUES_(op, 0);
            await dc.ExecuteAsync(p => op.Write(p));

            return job;
        }

        /// <summary>
        /// To push a jpb one step forward by either creating or reactivating a next step.
        /// </summary>
        public static async Task FlowForthAsync(this DbContext dc, long job, short curstep, string acct_safe, short npeerid, string nacct, string nname, string descr = null, decimal amt = 0.0M, JObj doc = null)
        {
            // update current step's status
            dc.Sql("UPDATE chain.ops SET status = ").T(FlowOp.STATUS_FORTHOUT).T(", npeerid = @1, nacct = @2, nname = @3 WHERE job = @4 AND step = @5 AND acct = @6 RETURNING acct, name, ldgr");
            await dc.QueryTopAsync(p => { p.SetOrNull(npeerid).Set(nacct).Set(nname).Set(job).Set(curstep).Set(acct_safe); });
            dc.Let(out string acct);
            dc.Let(out string name);
            dc.Let(out string ldgr);

            // create or update next step
            var op = new FlowOp()
            {
                job = job,
                step = (short) (curstep + 1),
                acct = nacct,
                name = nname,
                ldgr = ldgr,
                descr = descr,
                amt = amt,
                doc = doc,
                stated = DateTime.Now,
                pacct = acct,
                pname = name,
                status = FlowOp.STATUS_FORTHIN
            };
            if (npeerid > 0) // remote call
            {
                var cli = GetChainClient(op.ppeerid);
                var status = await cli.RemoteForthAsync(op);
                if (status != 201 && status != 200)
                {
                    dc.Rollback();
                }
            }
            else
            {
                dc.Sql("INSERT INTO chain.ops ").colset(FlowOp.Empty, 0)._VALUES_(FlowOp.Empty, 0).T(" ON CONFLICT (job, step) DO UPDATE SET status = ").T(FlowOp.STATUS_FORTHIN);
                await dc.ExecuteAsync(p => op.Write(p));
            }
        }

        /// <summary>
        /// To push a jpb one step backward.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="job">job number which is unique in a network</param>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="ChainException"></exception>
        public static async Task FlowBackAsync(this DbContext dc, long job, short step)
        {
            if (!await dc.QueryTopAsync("SELECT step, ppeerid FROM chain.ops WHERE job = @1 ORDER BY step DESC", p => p.Set(job)))
            {
                throw new ChainException();
            }
            dc.Let(out step);
            dc.Let(out short ppeerid);
            if (ppeerid == 0) // this node
            {
                dc.Sql("UPDATE chain.ops SET ").colset(FlowOp.Empty)._VALUES_(FlowOp.Empty);
                await dc.ExecuteAsync(p => p.Set(job));
            }
            else // remote call
            {
                var cli = GetChainClient(ppeerid);
                var status = await cli.RemoteBackAsync(job, step);
                if (status != 201)
                {
                    dc.Rollback();
                }
            }
        }

        public static async Task FlowAbortAsync(this DbContext dc, string fn, short step, string descr, decimal amt, JObj doc = null)
        {
        }

        public static async Task FlowEndAsync(this DbContext dc, long job, short curstep, string acct_safe, JObj doc = null)
        {
            // end the previous step
            if (curstep > 1)
            {
                // locate the end op
                dc.Sql("SELECT ppeerid, pacct FROM chain.ops WHERE job = @1 AND step = @2");
                await dc.QueryTopAsync(p => p.Set(job).Set(curstep));
                dc.Let(out short ppeerid);
                dc.Let(out string pacct);
                if (ppeerid > 0)
                {
                    var cli = GetChainClient(ppeerid);
                    await cli.RemoteEndAsync(job, (short) (curstep - 1));
                }
                else
                {
                    await dc.FlowEndAsync(job, (short) (curstep - 1), pacct);
                }
            }

            // end this step
            dc.Sql("UPDATE chain.ops SET status = ").T(FlowOp.STATUS_ENDED).T(", doc = @1, stamp = @2 WHERE job = @3 AND step = @4 AND acct = @5");
            await dc.ExecuteAsync(p => p.Set(doc).SetMoment().Set(job).Set(curstep).Set(acct_safe));
        }
    }
}