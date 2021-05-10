using System;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public static class ChainUtility
    {
        public const string ACCT_SYS = "SYS";

        public const string X_FROM = "X-From";

        public const string X_PEER_ID = "X-Peer-ID";

        public const string X_BLOCK_ID = "X-Block-ID";

        public const string X_ACCT = "X-Account";

        public const string X_NAME = "X-Name";

        public const string X_TIP = "X-Tip";

        public const string X_AMT = "X-Amount";


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

        public static ChainPeer[] AllPeers(this DbContext dc)
        {
            dc.Sql("SELECT ").collst(ChainPeer.Empty).T(" FROM chain.peers");
            return dc.Query<ChainPeer>();
        }

        public static async Task<ChainArch> GetArchTopAsync(this DbContext dc, string acct)
        {
            dc.Sql("SELECT ").collst(ChainArch.Empty).T(" FROM chain.blocks WHERE acct = @1 AND ORDER BY seq DESC LIMIT 1");
            return await dc.QueryTopAsync<ChainArch>(p => p.Set(acct));
        }

        /// <summary>
        /// To retrieve a page of archived records for the specified account & ledger. It may across peers.
        /// </summary>
        public static async Task<ChainArch[]> GetArchAsync(this DbContext dc, string acct, int limit = 20, int page = 0)
        {
            if (acct == null)
            {
                return null;
            }
            dc.Sql("SELECT ").collst(ChainArch.Empty).T(" FROM chain.blocks WHERE peerid = @1 AND acct = @2 ORDER BY seq DESC LIMIT @4 OFFSET @3 * @4");
            return await dc.QueryAsync<ChainArch>(p => p.Set(ChainEnviron.Info.id).Set(acct).Set(limit).Set(page));
        }

        public static async Task<ChainOp[]> GetOpAsync(this DbContext dc, string acct)
        {
            if (acct == null)
            {
                return null;
            }
            dc.Sql("SELECT ").collst(ChainOp.Empty).T(" FROM chain.ops_vw WHERE acct = @1 ORDER BY stamp");
            return await dc.QueryAsync<ChainOp>(p => p.Set(acct));
        }
    }
}