using System;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public static class ChainUtility
    {
        public const string X_FROM = "X-From";

        public const string X_PEER_ID = "X-Peer-ID";

        public const string X_BLOCK_ID = "X-Block-ID";

        public const string X_ACCOUNT = "X-Account";

        public const string X_NAME = "X-Name";

        public const string X_TIP = "X-Tip";

        public const string X_AMOUNT = "X-Amount";


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

        public static async Task<long> RecordAsync(this DbContext dc, string acct, string name, string tip, decimal amt)
        {
            // insert
            var obj = new Queuel()
            {
                acct = acct,
                name = name,
                tip = tip,
                amt = amt,
                stamp = DateTime.Now,
                status = 0,
            };
            dc.Sql("INSERT INTO chain.journals ").colset(obj, 0)._VALUES_(obj, 0).T(" RETURNING id");
            return (int) await dc.ScalarAsync(p => obj.Write(p));
        }

        public static async Task<Peer[]> GetPeersAsync(this DbContext dc)
        {
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
            return await dc.QueryAsync<Peer>();
        }

        public static async Task<Archival> GetArchivalAsync(this DbContext dc, string acct)
        {
            dc.Sql("SELECT ").collst(Archival.Empty).T(" FROM chain.archivals WHERE acct = @1 AND ORDER BY seq DESC LIMIT 1");
            return await dc.QueryTopAsync<Archival>(p => p.Set(acct));
        }

        /// <summary>
        /// To retrieve a page of archived records for the specified account & ledger. It may across peers.
        /// </summary>
        public static async Task<Archival[]> GetArchivalsAsync(this DbContext dc, string acct, int limit = 20, int page = 0)
        {
            if (acct == null)
            {
                return null;
            }
            dc.Sql("SELECT ").collst(Archival.Empty).T(" FROM chain.archivals WHERE peerid = @1 AND acct = @2 ORDER BY seq DESC LIMIT @4 OFFSET @3 * @4");
            return await dc.QueryAsync<Archival>(p => p.Set(ChainEnviron.Info.id).Set(acct).Set(limit).Set(page));
        }

        public static async Task<Queuel[]> GetQueuelsAsync(this DbContext dc, string acct)
        {
            if (acct == null)
            {
                return null;
            }
            dc.Sql("SELECT ").collst(Queuel.Empty).T(" FROM chain.ques_vw WHERE acct = @1");
            return await dc.QueryAsync<Queuel>(p => p.Set(acct));
        }
    }
}