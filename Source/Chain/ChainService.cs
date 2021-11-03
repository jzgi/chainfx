using System.Data;
using System.Threading.Tasks;
using SkyChain;
using SkyChain.Web;
using static SkyChain.Chain.ChainUtility;

namespace SkyChain.Chain
{
    /// <summary>
    /// A web service that realizes inter-peer communication. 
    /// </summary>
    public class ChainService : WebService
    {
        public async Task<bool> onop(WebContext wc)
        {
            long id = 0;
            IsolationLevel level = 0;

            var f = await wc.ReadAsync<JObj>();
            string op = f[nameof(op)];

            var o = Application.Drive.GetOp(op);

            var ctx = Chain.AcquireSlave(id, level);
            if (o.IsAsync)
            {
                return await o.DoAsync(ctx);
            }
            else
            {
                return o.Do(ctx);
            }
        }

        /// <summary>
        /// Try to return a block of data.
        /// </summary>
        public async Task onpoll(WebContext wc)
        {
            // veriify 
            var peerid = Chain.Info.id;
            if (wc.HeaderShort(X_PEER_ID) != peerid)
            {
                wc.Give(409); // conflict
            }

            var blockid = wc.HeaderInt(X_BLOCK_ID); // asked block id
            if (!blockid.HasValue)
            {
                wc.Give(400); // bad request
                return;
            }

            using var dc = NewDbContext();
            dc.Sql("SELECT * FROM chain.archive WHERE peerid = @1 AND seq >= @2 AND seq < @2 + 1000 ORDER BY seq");
            var arr = await dc.QueryAsync<Archival>(p => p.Set(peerid).Set(WeaveSeq(blockid.Value, 0)));
            var j = new JsonContent(true, 1024 * 256);
            try
            {
                j.ARR_();
                foreach (var o in arr)
                {
                    j.OBJ_();
                    // j.Put(nameof(o.seq), o.seq);
                    // j.Put(nameof(o.acct), o.acct);
                    // j.Put(nameof(o.name), o.name);
                    // j.Put(nameof(o.remark), o.remark);
                    // j.Put(nameof(o.amt), o.amt);
                    // j.Put(nameof(o.bal), o.bal);
                    // j.Put(nameof(o.stamp), o.stamp);
                    // j.Put(nameof(o.cs), o.cs);
                    // j.Put(nameof(o.blockcs), o.blockcs);
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