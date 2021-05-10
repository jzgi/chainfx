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
            var pid = ChainEnviron.Info.id;
            if (wc.HeaderShort(X_PEER_ID) != pid)
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
            dc.Sql("SELECT ").collst(ChainArch.Empty, 0xff).T(" FROM chain.blocks WHERE peerid = @1 AND seq >= @2 AND seq < @2 + 1000 ORDER BY seq");
            var arr = await dc.QueryAsync<ChainArch>(p => p.Set(pid).Set(WeaveSeq(blockid.Value, 0)));
            var j = new JsonContent(true, 1024 * 256);
            try
            {
                j.ARR_();
                foreach (var o in arr)
                {
                    j.OBJ_();
                    j.Put(nameof(o.seq), o.seq);
                    j.Put(nameof(o.acct), o.acct);
                    j.Put(nameof(o.name), o.name);
                    j.Put(nameof(o.tip), o.tip);
                    j.Put(nameof(o.amt), o.amt);
                    j.Put(nameof(o.bal), o.bal);
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
    }
}