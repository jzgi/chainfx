using System;
using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Db.ChainUtility;

namespace SkyChain.Db
{
    /// <summary>
    /// A web service that realizes API for inter-peer communication. 
    /// </summary>
    public class ChainService : WebService
    {
        /// <summary>
        /// Try to return a block of data.
        /// </summary>
        public async Task onpoll(WebContext wc)
        {
            // veriify consistency of peer numbering
            var peerid = ChainEnviron.Info.id;
            if (wc.HeaderShort(X_PEER_ID) != peerid)
            {
                wc.Give(409); // conflict
            }

            var blockid = wc.HeaderInt(X_BLOCK_ID); // desired block id
            if (!blockid.HasValue)
            {
                wc.Give(400); // bad request
                return;
            }

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(FlowAr.Empty, 0xff).T(" FROM chain.blocks WHERE peerid = @1 AND seq >= @2 AND seq < @2 + 1000 ORDER BY seq");
            var arr = await dc.QueryAsync<FlowAr>(p => p.Set(peerid).Set(WeaveSeq(blockid.Value, 0)));
            var j = new JsonContent(true, 1024 * 256);
            try
            {
                j.ARR_();
                foreach (var o in arr)
                {
                    j.OBJ_();
                    j.Put(nameof(o.seq), o.seq);
                    j.Put(nameof(o.job), o.job);
                    j.Put(nameof(o.step), o.step);
                    j.Put(nameof(o.acct), o.acct);
                    j.Put(nameof(o.name), o.name);
                    j.Put(nameof(o.ldgr), o.ldgr);
                    j.Put(nameof(o.descr), o.descr);
                    j.Put(nameof(o.amt), o.amt);
                    j.Put(nameof(o.bal), o.bal);
                    j.Put(nameof(o.doc), o.doc);
                    j.Put(nameof(o.stated), o.stated);
                    j.Put(nameof(o.stamp), o.stamp);
                    j.Put(nameof(o.cs), o.cs);
                    j.Put(nameof(o.blockcs), o.blockcs);
                    j._OBJ();
                }
                j._ARR();
            }
            finally
            {
                j.Clear();
            }

            wc.Give(200, j);
        }


        /// <summary>
        /// To push the job one step forward, create the op record if not existing.
        /// </summary>
        public async Task onforth(WebContext wc)
        {
            // veriify consistency of peer numbering
            var peerid = ChainEnviron.Info.id;
            if (wc.HeaderShort(X_PEER_ID) != peerid)
            {
                wc.Give(409); // conflict
                return;
            }

            // properties form headers and body
            var job = wc.HeaderLong(X_JOB);
            var step = wc.HeaderShort(X_STEP);
            var acct = wc.Header(X_ACCT);
            var name = wc.Header(X_NAME);
            var ldgr = wc.Header(X_LDGR);
            var descr = wc.Header(X_AMT);
            var amt = wc.HeaderDecimal(X_AMT);
            if (job == null || step == null || acct == null)
            {
                wc.Give(400); // bad request
                return;
            }

            var doc = await wc.ReadAsync<JObj>();

            var o = new FlowOp()
            {
                job = job.Value,
                step = step.Value,
                acct = acct,
                name = name,
                ldgr = ldgr,
                descr = descr,
                amt = amt ?? 0.00M,
                doc = doc,
                stated = DateTime.Now,
                status = FlowOp.STATUS_FORTHIN
            };
            using var dc = NewDbContext();
            dc.Sql("INSERT INTO chain.ops").colset(FlowOp.Empty, 0)._VALUES_(FlowOp.Empty, 0);
            await dc.ExecuteAsync(p => o.Write(p));

            wc.Give(201); // created
        }

        public async Task onback(WebContext wc)
        {
            // veriify consistency of peer numbering
            var peerid = ChainEnviron.Info.id;
            if (wc.HeaderShort(X_PEER_ID) != peerid)
            {
                wc.Give(409); // conflict
            }

            // properties form headers and body
            var job = wc.HeaderLong(X_JOB);
            var step = wc.HeaderShort(X_STEP);
            var acct = wc.Header(X_ACCT);
            var name = wc.Header(X_NAME);
            var ldgr = wc.Header(X_LDGR);
            var descr = wc.Header(X_AMT);
            var amt = wc.HeaderDecimal(X_AMT);
            if (job == null || step == null || acct == null)
            {
                wc.Give(400); // bad request
                return;
            }

            var doc = await wc.ReadAsync<JObj>();

            var o = new FlowOp()
            {
                job = job.Value,
                step = step.Value,
                acct = acct,
                name = name,
                ldgr = ldgr,
                descr = descr,
                amt = amt ?? 0.00M,
                doc = doc,
                stated = DateTime.Now,
                status = FlowOp.STATUS_FORTHIN
            };
            using var dc = NewDbContext();
            dc.Sql("INSERT INTO chain.ops").colset(FlowOp.Empty, 0)._VALUES_(FlowOp.Empty, 0);
            await dc.ExecuteAsync(p => o.Write(p));

            wc.Give(201); // created
        }

        public async Task onabort(WebContext wc)
        {
            // veriify consistency of peer numbering
            var peerid = ChainEnviron.Info.id;
            if (wc.HeaderShort(X_PEER_ID) != peerid)
            {
                wc.Give(409); // conflict
            }

            // properties form headers
            var job = wc.HeaderLong(X_JOB);
            var step = wc.HeaderShort(X_STEP);
            var acct = wc.Header(X_ACCT);
            if (job == null || step == null || acct == null)
            {
                wc.Give(400); // bad request
                return;
            }

            try
            {
                using var dc = NewDbContext();
                dc.Sql("UPDATE chain.ops SET status = ").T(FlowOp.STATUS_ABORTED).T(" WHERE status = ").T(FlowOp.STATUS_FORTHOUT).T(" AND job = @1 AND step = @2 AND acct = @3");
                if (await dc.ExecuteAsync(p => p.Set(job.Value).Set(step.Value).Set(acct)) == 1)
                {
                    wc.Give(200);
                }
                else
                {
                    dc.Rollback();
                    wc.Give(500);
                }
            }
            catch (Exception e)
            {
                wc.Give(500);
            }
        }

        public async Task onend(WebContext wc)
        {
            // veriify consistency of peer numbering
            var peerid = ChainEnviron.Info.id;
            if (wc.HeaderShort(X_PEER_ID) != peerid)
            {
                wc.Give(409); // conflict
            }

            // properties form headers
            var job = wc.HeaderLong(X_JOB);
            var step = wc.HeaderShort(X_STEP);
            var acct = wc.Header(X_ACCT);
            if (job == null || step == null || acct == null)
            {
                wc.Give(400); // bad request
                return;
            }

            try
            {
                using var dc = NewDbContext();
                dc.Sql("UPDATE chain.ops SET status = ").T(FlowOp.STATUS_ENDED).T(" WHERE status = ").T(FlowOp.STATUS_FORTHOUT).T(" AND job = @1 AND step = @2 AND acct = @3 RETURNING ppeerid, pacct");
                var res = await dc.ExecuteAsync(p => p.Set(job.Value).Set(step.Value).Set(acct));
                dc.Let(out short ppeerid);
                dc.Let(out string pacct);

                // recursively call job ending for this peer
                if (step > 1 && pacct != null)
                {
                    await dc.FlowEndAsync(job.Value, (short) (step.Value - 1), null);
                }

                if (res == 1)
                {
                    wc.Give(200);
                }
                else
                {
                    dc.Rollback();
                    wc.Give(500);
                }
            }
            catch (Exception e)
            {
                wc.Give(500);
            }
        }
    }
}