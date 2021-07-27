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

        public const string X_REMARK = "X-Remark";

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

        public static async Task<long> EnterAsync(this DbContext dc, string acct, string name, string tip, decimal amt)
        {
            // insert
            var obj = new OperativeRow()
            {
                acct = acct,
                name = name,
                remark = tip,
                amt = amt,
                stamp = DateTime.Now,
                status = 0,
            };
            dc.Sql("INSERT INTO chain.queue ").colset(obj, 0)._VALUES_(obj, 0).T(" RETURNING id");
            return (int) await dc.ScalarAsync(p => obj.Write(p));
        }

        public static async Task<Peer[]> GetPeersAsync(this DbContext dc)
        {
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers");
            return await dc.QueryAsync<Peer>();
        }

        public static async Task<Archival> GetArchiveAsync(this DbContext dc, short typ, string acct)
        {
            dc.Sql("SELECT ").collst(Archival.Empty).T(" FROM chain.archive WHERE typ = @1 AND acct = @1 ORDER BY seq DESC LIMIT 1");
            return await dc.QueryTopAsync<Archival>(p => p.Set(typ).Set(acct));
        }

        /// <summary>
        /// To retrieve a page of archive records for the specified account & ledger. It may across peers.
        /// </summary>
        public static async Task<Archival[]> SeekArchiveAsync(this DbContext dc, short typ, string acct, int limit = 20, int page = 0)
        {
            if (acct == null)
            {
                return null;
            }
            dc.Sql("SELECT ").collst(Archival.Empty).T(" FROM chain.archive WHERE peerid = @1 AND acct = @2 ORDER BY seq DESC LIMIT @4 OFFSET @3 * @4");
            return await dc.QueryAsync<Archival>(p => p.Set(ChainEnviron.Info.id).Set(acct).Set(limit).Set(page));
        }

        public static async Task<OperativeRow[]> SeekQueueAsync(this DbContext dc, string acct)
        {
            if (acct == null)
            {
                return null;
            }
            dc.Sql("SELECT ").collst(OperativeRow.Empty).T(" FROM chain.ques_vw WHERE acct = @1");
            return await dc.QueryAsync<OperativeRow>(p => p.Set(acct));
        }

        // remote sql
        public static async Task<D[]> PeerQueryAsync<D>(this DbContext db, short peerid, string sql, Action<IParameters> p = null, bool prepare = true) where D : IData
        {
            if (peerid >= 0)
            {
                if (peerid == 0 || ChainEnviron.Info.id == peerid) // local
                {
                    return null;
                }
                else
                {
                    var conn = ChainEnviron.GetClient(peerid);
                }
            }
            return null;
        }
    }
}