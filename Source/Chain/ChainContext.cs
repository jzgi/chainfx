using System;
using System.Threading.Tasks;

namespace SkyChain.Chain
{
    public class ChainContext : DbContext
    {
        short id;

        readonly ChainClient client;

        internal Peer local;

        internal Peer remote;

        public JObj In { get; set; }

        public JObj Out;

        internal ChainContext(DbSource dbsource, ChainClient client) : base(dbsource)
        {
            this.client = client;
        }


        public async Task<bool> CallAsync(short peerid, string op, Action<IParameters> p = null, short proj = 0x0fff)
        {
            if (peerid == 0 || peerid == local.id) // call in- place
            {
                // local
            }
            else // call remote
            {
                var conn = Chain.GetClient(peerid);
                if (conn != null)
                {
                    // args
                    var cnt = new JsonContent(true, 1024);
                    cnt.Put(null, In);

                    // remote call
                    var (code, v) = await conn.CallAsync(id, 0, op, cnt);
                }
                else
                {
                    throw new ChainException("");
                }
            }
            return false;
        }
    }
}