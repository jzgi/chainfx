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
            var jc = new JsonContent(1024 * 256, true);
            try
            {
            }
            finally
            {
                ArrayUtility.Return(jc.Buffer);
            }
            jc.Put(null, recs);

            // return 
            wc.Give(200, jc);
        }

        public void input(WebContext wc)
        {
            // get input data fields
            var cc = new ChainContext();
            Operation op = new Operation();

            // find typ
            var def = ChainEnv.GetFlow(op.typ);

            var act = def.GetActivity(op.step);

            act.OnInput(cc, null);
        }
    }
}