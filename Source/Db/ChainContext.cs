using System;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainContext : DbContext
    {
        internal Peer info;

        short id;

        JObj sqlpack;

        bool remotg;

        short remoteid;


        internal ChainContext(DbSource src) : base(src)
        {
        }


        public async Task<D> QueryTopAsync<D>(short peerid, string sql, Action<IParameters> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (peerid == 0 || peerid == info.id)
            {
                // local
                return await QueryTopAsync<D>(sql, p, proj, prepare);
            }
            else
            {
                var conn = ChainEnviron.GetConnect(peerid);
                if (conn != null)
                {
                    var (code, v) = await conn.QueryTopAsync<D>(sql, p, prepare);
                    return v;
                }
                else
                {
                    throw new ChainException("");
                }
            }
        }
        public async Task<D[]> QueryAsync<D>(short peerid, string sql, Action<IParameters> p = null, byte proj = 0x0f, bool prepare = true) where D : IData, new()
        {
            if (peerid == 0 || peerid == info.id)
            {
                // local
                return await QueryAsync<D>(sql, p, proj, prepare);
            }
            else
            {
                var conn = ChainEnviron.GetConnect(peerid);
                if (conn != null)
                {
                    var (code, v) = await conn.QueryAsync<D>(sql, p, prepare);
                    return v;
                }
                else
                {
                    throw new ChainException("");
                }
            }
        }
    }
}