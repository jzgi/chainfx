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


        public static Peer[] FindPeers(this DbContext dc)
        {
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
            return dc.Query<Peer>();
        }

        /// <summary>
        /// To retrieve all archived steps for the specified job. It may across peers.
        /// </summary>
        public static async Task<Archival[]> FindJobAsync(this DbContext dc, long job)
        {
            dc.Sql("SELECT ").collst(Archival.Empty).T(" FROM chain.blocks WHERE job = @1 ORDER BY step");
            return await dc.QueryAsync<Archival>(p => p.Set(job));
        }

        public static async Task<Archival> FindLastAsync(this DbContext dc, string acct, string ldgr)
        {
            dc.Sql("SELECT ").collst(Archival.Empty).T(" FROM chain.blocks WHERE acct = @1 AND ldgr = @2 ORDER BY seq DESC LIMIT 1");
            return await dc.QueryTopAsync<Archival>(p => p.Set(acct).Set(ldgr));
        }

        /// <summary>
        /// To retrieve a page of archived records for the specified account & ledger. It may across peers.
        /// </summary>
        public static async Task<Archival[]> FindJournalAsync(this DbContext dc, string acct, string ldgr, int limit = 20, int page = 0)
        {
            dc.Sql("SELECT ").collst(Archival.Empty).T(" FROM chain.blocks WHERE peerid = @1 AND acct = @2 AND ldgr LIKE @3 ORDER BY seq DESC LIMIT @4 OFFSET @4 * @5");
            return await dc.QueryAsync<Archival>(p => p.Set(Info.id).Set(acct).Set(ldgr + "%").Set(limit).Set(page));
        }

        public static async Task<Operational> PeekAsync(this DbContext dc, long job, short step)
        {
            dc.Sql("SELECT ").collst(Operational.Empty).T(" FROM chain.ops WHERE job = @1 AND step = @2");
            return await dc.QueryTopAsync<Operational>(p => p.Set(job).Set(step));
        }

        public static async Task<Operational[]> PeekAsync(this DbContext dc, string acct, string ldgr)
        {
            dc.Sql("SELECT ").collst(Operational.Empty).T(" FROM chain.ops WHERE acct = @1 AND ldgr LIKE @2");
            return await dc.QueryAsync<Operational>(p => p.Set(acct).Set(ldgr + "%"));
        }

        public static async Task<Operational[]> PeekAsync(this DbContext dc, long job)
        {
            dc.Sql("SELECT ").collst(Operational.Empty).T(" FROM chain.ops WHERE job = @1 ORDER BY step");
            return await dc.QueryAsync<Operational>(p => p.Set(job));
        }

        /// <summary>
        /// To start a job and create its first step.
        /// </summary>
        /// <returns></returns>
        public static async Task<long> JobStartAsync(this DbContext dc, string acct, string name, string ldgr, string descr, decimal amt, JObj doc = null)
        {
            // job number is unique
            await dc.QueryAsync("SELECT nextval('chain.jobseq')");
            dc.Let(out int jobseq);
            long job = ((long) Info.id << 32) + jobseq;

            // insert
            var op = new Operational()
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
                status = Operational.STARTED
            };
            dc.Sql("INSERT INTO chain.ops ").colset(op, 0)._VALUES_(op, 0);
            await dc.ExecuteAsync(p => op.Write(p));

            return job;
        }

        /// <summary>
        /// To push a jpb one step forward by either creating or reactivating a next step.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="job">job number which is unique in a network</param>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="ChainException"></exception>
        public static async Task JobForthAsync(this DbContext dc, long job, short step, string acct_safe, short npeerid, string nacct, string nname, decimal amt, JObj doc = null)
        {
            // update current step's status
            dc.Sql("UPDATE chain.ops SET status = ").T(Operational.FORTH_OUT).T(", npeerid = @1, nacct = @2, nname = @3 WHERE job = @4 AND step = @5 RETURNING ldgr");
            await dc.QueryTopAsync(p => p.Set(npeerid).Set(nacct).Set(nname).Set(job).Set(step));
            dc.Let(out string ldgr);

            // create or update next step
            var op = new Operational()
            {
                job = job,
                step = (short) (step + 1),
                acct = nacct,
                name = nname,
                ldgr = ldgr,
                amt = amt,
                descr = null,
                doc = doc,
                stated = DateTime.Now,
                status = Operational.STARTED
            };
            if (npeerid == 0)
            {
                dc.Sql("INSERT INTO chain.logs ").colset(Operational.Empty, 0)._VALUES_(Operational.Empty, 0).T(" ON CONFLICT (job, step) DO UPDATE SET status = ").T(Operational.FORTH_IN);
                await dc.ExecuteAsync(p => op.Write(p));
            }
            else // remote call
            {
                var cli = GetChainClient(op.ppeerid);
                var status = await cli.RemoteForthAsync(op);
                if (status != 201 && status != 200)
                {
                    dc.Rollback();
                }
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
        public static async Task JobBackAsync(this DbContext dc, long job, short step)
        {
            if (!await dc.QueryTopAsync("SELECT step, ppeerid FROM chain.ops WHERE job = @1 ORDER BY step DESC", p => p.Set(job)))
            {
                throw new ChainException();
            }
            dc.Let(out step);
            dc.Let(out short ppeerid);
            if (ppeerid == 0) // this node
            {
                dc.Sql("UPDATE chain.ops SET ").colset(Operational.Empty)._VALUES_(Operational.Empty);
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
                    var cli = GetChainClient(ppeerid);
                    await cli.RemoteEndAsync(job, (short) (curstep - 1));
                }
                else
                {
                    await dc.JobEndAsync(job, (short) (curstep - 1));
                }
            }

            // end this step
            dc.Sql("UPDATE chain.ops SET status = ").T(Operational.ENDED).T(", doc = @1 WHERE job = @2 AND step = @3");
            await dc.ExecuteAsync(p => p.Set(doc).Set(job).Set(curstep));
        }
    }
}