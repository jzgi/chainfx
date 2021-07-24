using System;
using System.Threading.Tasks;

namespace SkyChain.Db
{
    public class ChainContext : DbContext
    {
        short id;

        JObj sqlpack;

        bool remotg;

        short remoteid;


        internal ChainContext(DbSource src) : base(src)
        {
        }


        public async Task<D[]> QueryAsync<D>(string sql, Action<IParameters> p = null, bool prepare = true) where D : IData, new()
        {
            return null;
        }
    }
}