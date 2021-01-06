using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Chain.Chain;

namespace SkyChain.Chain
{
    /// <summary>
    /// A web service that realizes API for inter-peer communication. 
    /// </summary>
    public class ChainService : WebService
    {
        /// <summary>
        /// Try to return a block newer than last one.
        /// </summary>
        public async Task onpoll(WebContext wc, int lastseq)
        {
            var peerid = ChainEnviron.Info.id;

            // check if existing
            using var dc = NewDbContext();
            if (!await dc.QueryTopAsync("SELECT seq, dgst, pdgst FROM chain.blocks WHERE peerid = @1 AND seq > @1 ORDER BY peerid, seq LIMIT 1", p => p.Set(peerid).Set(lastseq)))
            {
                wc.Give(204); // no content
                return;
            }
            dc.Let(out int seq);
            dc.Let(out int dgst);
            dc.Let(out int pdgst);

            // load block states
            dc.Sql("SELECT ").collst(BlockOp.Empty, 0xff).T(" FROM chain.blockrcs WHERE peerid = @1 AND seq = @2");
            await dc.QueryAsync(p => p.Set(peerid).Set(seq));

            // putting into content
            var j = new JsonContent(true, 1024 * 256);
            try
            {
                j.ARR_();
                while (dc.Next())
                {
                    var o = dc.ToObject<BlockOp>(0xff);
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
                    j.Put(nameof(o.dgst), o.dgst);
                    j._OBJ();
                }
                j._ARR();
            }
            finally
            {
                j.Clear();
            }

            wc.SetHeader(X_SEQ, seq);
            wc.SetHeader(X_DIGEST, dgst);
            wc.SetHeader(X_PREV_DIGEST, pdgst);
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

        public virtual bool onforth(WebContext wc, int fromstep)
        {
            return true;
        }

        public virtual bool onback(WebContext wc, int fromstep)
        {
            return true;
        }

        public virtual bool oncancel(WebContext wc, int fromstep)
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