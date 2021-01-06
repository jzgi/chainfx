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

        internal static async Task<BlockOp[]> FindBlockOpsAsync(this DbContext dc, long job)
        {
            dc.Sql("SELECT ").collst(BlockOp.Empty).T(" FROM chain.blockops WHERE job = @1 ORDER BY step");
            return await dc.QueryAsync<BlockOp>(p => p.Set(job));
        }

        public static async Task<Map<long, BlockOp>> FindBlockOpsAsync(this DbContext dc, string acct, string ldgr, int limit = 20, int offset = 0)
        {
            var map = new Map<long, BlockOp>();
            dc.Sql("SELECT ").collst(BlockOp.Empty).T(" FROM chain.blockops WHERE acct = @1 AND ldgr LIKE @2 ORDER BY stated DESC LIMIT @3 OFFSET @3 * @4");
            await dc.QueryAsync(p => p.Set(acct).Set(ldgr + "%").Set(limit).Set(offset));
            BlockOp o;
            while (dc.Next())
            {
                o = dc.ToObject<BlockOp>();
                map.Add(o);
            }
            // query again to see any rest steps
            dc.Sql("SELECT ").collst(BlockOp.Empty).T(" FROM chain.blockops WHERE acct = @1 AND ldgr LIKE @2 AND job = @3 AND step > @4 ORDER BY stated");


            return null;
        }

        public static async Task<Op[]> FindOpsAsync(this DbContext dc, string acct, string ldgr)
        {
            dc.Sql("SELECT ").collst(Op.Empty).T(" FROM chain.ops WHERE acct = @1 AND ldgr LIKE @2");
            return await dc.QueryAsync<Op>(p => p.Set(acct).Set(ldgr + "%"));
        }

        public static async Task<Op> FindOpAsync(this DbContext dc, long job, short step)
        {
            dc.Sql("SELECT ").collst(Op.Empty).T(" FROM chain.ops WHERE job = @1 AND step = @2");
            return await dc.QueryTopAsync<Op>(p => p.Set(job).Set(step));
        }

        public static async Task<Op[]> FindOpsAsync(this DbContext dc, long job)
        {
            dc.Sql("SELECT ").collst(Op.Empty).T(" FROM chain.ops WHERE job = @1 ORDER BY step");
            return await dc.QueryAsync<Op>(p => p.Set(job));
        }

        /// <summary>
        /// To start a job by creating its first step.
        /// </summary>
        /// <returns></returns>
        public static async Task<Op> JobStartAsync(this DbContext dc, string acct, string name, string ldgr, string descr, decimal amt, JObj doc = null)
        {
            // job number is unique
            await dc.QueryAsync("SELECT nextval('chain.jobseq')");
            dc.Let(out int jobseq);
            long job = ((long) ChainEnviron.Info.id << 32) + jobseq;

            // insert
            var op = new Op()
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
                status = Op.STARTED
            };

            // data access
            dc.Sql("INSERT INTO chain.ops ").colset(op, 0)._VALUES_(op, 0);
            await dc.ExecuteAsync(p => op.Write(p));

            return op;
        }

        public static async Task JobForwardAsync(this DbContext dc, long job, short npeerid, string nacct, string nname, decimal amt, JObj doc = null)
        {
            if (!await dc.QueryTopAsync("SELECT step FROM chain.ops WHERE job = @1 ORDER BY step DESC", p => p.Set(job)))
            {
                throw new ChainException();
            }
            dc.Let(out short step);

            dc.Sql("UPDATE chain.ops SET status = ").T(Op.FORWARD_IN).T(", npeer = @1, nacct = @2, nname = @3 WHERE job = @4 AND step = @5 RETURNING ldgr");
            await dc.QueryTopAsync(p => p.Set(npeerid).Set(nacct).Set(nname).Set(job).Set(step));
            dc.Let(out string ldgr);

            var o = new Op()
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
                status = Op.STARTED
            };
            if (npeerid == 0)
            {
                dc.Sql("INSERT INTO chain.logs ").colset(Op.Empty)._VALUES_(Op.Empty);
                await dc.ExecuteAsync(p => o.Write(p));
            }
            else // remote call
            {
                var cli = ChainEnviron.GetChainClient(o.ppeerid);
                var status = await cli.CallJobForwardAsync(job, o.step, o.Acct, o.name, o.ldgr);
                if (status != 201)
                {
                    dc.Rollback();
                }
            }
        }

        public static async Task JobBackwardAsync(this DbContext dc, long job, short step)
        {
            if (!await dc.QueryTopAsync("SELECT step, ppeerid FROM chain.ops WHERE job = @1 ORDER BY step DESC", p => p.Set(job)))
            {
                throw new ChainException();
            }
            dc.Let(out step);
            dc.Let(out short ppeerid);
            if (ppeerid == 0) // this node
            {
                dc.Sql("UPDATE chain.ops SET ").colset(Op.Empty)._VALUES_(Op.Empty);
                await dc.ExecuteAsync(p => p.Set(job));
            }
            else // remote call
            {
                var cli = ChainEnviron.GetChainClient(ppeerid);
                var status = await cli.CallJobBackwardAsync(job, step);
                if (status != 201)
                {
                    dc.Rollback();
                }
            }
        }

        public static async Task JobAbortAsync(this DbContext dc, string fn, short step, string descr, decimal amt, JObj doc = null)
        {
        }

        public static async Task JobEndAsync(this DbContext dc, long job, short curstep, JObj doc = null)
        {
            // locate the end op
            dc.Sql("SELECT ppeerid FROM chain.ops WHERE job = @1");
            await dc.QueryTopAsync(p => p.Set(job));
            dc.Let(out short ppeerid);

            // end the previous step
            if (curstep > 1)
            {
                if (ppeerid > 0)
                {
                    var cli = ChainEnviron.GetChainClient(ppeerid);
                    await cli.CallJobEndAsync(job, (short) (curstep - 1));
                }
                else
                {
                    await dc.JobEndAsync(job, (short) (curstep - 1));
                }
            }

            // end this step
            dc.Sql("UPDATE chain.ops SET status = ").T(Op.ENDED).T(", doc = @1 WHERE job = @2 AND step = @3");
            await dc.ExecuteAsync(p => p.Set(doc).Set(job).Set(curstep));
        }
    }
}