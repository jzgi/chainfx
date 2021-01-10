using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Chain.ChainUtility;
using static SkyChain.Chain.IChain;

namespace SkyChain.Chain
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
            var peerid = ChainEnviron.Info.id;
            var blockid = wc.HeaderInt(X_BLOCK_ID); // desired block id
            if (!blockid.HasValue)
            {
                wc.Give(400);
                return;
            }

            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Arch.Empty, 0xff).T(" FROM chain.blocks WHERE peerid = @1 AND seq >= @2 AND seq < @3 ORDER BY seq");
            var arr = await dc.QueryAsync<Arch>(p => p.Set(peerid).Set(WeaveSeq(blockid.Value, 0)).Set(WeaveSeq(blockid.Value, short.MaxValue)));
            var j = new JsonContent(true, 1024 * 256);
            try
            {
                j.ARR_();
                foreach (var o in arr)
                {
                    j.OBJ_();
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
                    j.Put(nameof(o.chk), o.chk);
                    j._OBJ();
                }
                j._ARR();
            }
            finally
            {
                j.Clear();
            }

            wc.SetHeader(X_PREV_DIGEST, arr[0].blockchk);
            wc.SetHeader(X_DIGEST, arr[^1].blockchk);

            wc.Give(200, j);
        }

        const int PIC_AGE = 3600 * 6;

        public void peericon(WebContext wc)
        {
            string peerid = wc.Query[nameof(peerid)];
            using var dc = NewDbContext();
            if (dc.QueryTop("SELECT icon FROM chain.peers WHERE id = @1", p => p.Set(peerid)))
            {
                dc.Let(out byte[] bytes);
                if (bytes == null) wc.Give(204); // no content 
                else wc.Give(200, new StaticContent(bytes), shared: true, maxage: PIC_AGE);
            }
            else wc.Give(404, shared: true, maxage: PIC_AGE); // not found
        }

        /// <summary>
        /// To push the job one step forward, create the op record if not existing.
        /// </summary>
        public bool onforth(WebContext wc)
        {
            var job = wc.HeaderLong(X_JOB);
            var step = wc.HeaderShort(X_STEP);
            
            using var dc = NewDbContext();
            dc.Sql("INSERT INTO chain.ops");
            
            return true;
        }

        public virtual bool onback(WebContext wc, int fromstep)
        {
            return true;
        }

        public virtual bool onabort(WebContext wc, int fromstep)
        {
            return true;
        }

        public async Task onend(WebContext wc)
        {
            var job = wc.HeaderLong(X_JOB);
            var step = wc.HeaderShort(X_STEP);

            if (!job.HasValue || !step.HasValue)
            {
                wc.Give(400); // bad request
                return;
            }

            using var dc = NewDbContext();
            await dc.JobEndAsync(job.Value, step.Value);

            wc.Give(200);
        }
    }
}