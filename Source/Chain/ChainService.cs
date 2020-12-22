using SkyChain.Web;

namespace SkyChain.Chain
{
    /// <summary>
    /// A web service that realizes API for both inter-node communication and 
    /// </summary>
    public class ChainService : WebService
    {
        public void forblock(WebContext wc)
        {
            // headers
            int lastid = 0;
            // query
            using var dc = NewDbContext();
            var b = dc.QueryTop<Block>("SELECT * FROM chain.blocks WHERE peerid = '&' AND id > @1 ORDER BY id LIMIT 1", p => p.Set(lastid));
            if (b == null)
            {
                wc.Give(204); // no content
                return;
            }

            // load block records
            var recs = dc.Query<Block>("SELECT * FROM chain.blockrecs WHERE peerid = '&' AND seq = @1", p => p.Set(b.seq));

            // putting into content
            var jc = new JsonContent(true, 1024 * 256);
            try
            {
            }
            finally
            {
                jc.Clear();
            }
            jc.Put(null, recs);

            // return 
            wc.Give(200, jc);
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

        public virtual bool ondone(WebContext wc, int fromstep)
        {
            return true;
        }
    }
}